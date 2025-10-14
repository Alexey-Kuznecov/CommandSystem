using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svetokop.Services
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Threading;
    using ScottPlot;
    using ScottPlot.Plottables;

    public static class ScottPlotReflectionHelpers
    {
        /// <summary>
        /// Попытаться обновить данные Scatter, используя reflection.
        /// Возвращает true при успехе. out message содержит диагностику.
        /// Если false, можно использовать fallback (пересоздание scatter).
        /// </summary>
        public static bool TryUpdateScatterData(Scatter scatter, double[] xs, double[] ys, Plot? plotForFallback, out string message)
        {
            message = "";
            if (scatter == null) { message = "scatter == null"; return false; }

            var t = scatter.GetType();
            var asm = t.Assembly;
            try
            {
                // 1) Попробуем найти публичный Replace(xs, ys)
                var replaceMethod = t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                     .FirstOrDefault(m =>
                                     {
                                         var ps = m.GetParameters();
                                         return m.Name == "Replace" && ps.Length == 2
                                                && ps[0].ParameterType.IsArray && ps[1].ParameterType.IsArray;
                                     });
                if (replaceMethod != null)
                {
                    replaceMethod.Invoke(scatter, new object[] { xs, ys });
                    message = "Called public Replace(xs, ys).";
                    return true;
                }

                // 2) Попробуем public Replace(Coordinates[])
                var coordsType = asm.GetTypes().FirstOrDefault(typ => typ.Name == "Coordinates");
                if (coordsType != null)
                {
                    var replaceCoords = t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                         .FirstOrDefault(m =>
                                         {
                                             var ps = m.GetParameters();
                                             return m.Name == "Replace" && ps.Length == 1 && ps[0].ParameterType.IsArray
                                                    && ps[0].ParameterType.GetElementType() == coordsType;
                                         });
                    if (replaceCoords != null)
                    {
                        // создадим массив Coordinates
                        var coordsArr = Array.CreateInstance(coordsType, xs.Length);
                        // попробуем найти конструктор (double x, double y)
                        var ctor = coordsType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                             .FirstOrDefault(c =>
                                             {
                                                 var p = c.GetParameters();
                                                 return p.Length == 2 && p[0].ParameterType == typeof(double) && p[1].ParameterType == typeof(double);
                                             });
                        if (ctor != null)
                        {
                            for (int i = 0; i < xs.Length; i++)
                            {
                                var coord = ctor.Invoke(new object[] { xs[i], ys[i] });
                                coordsArr.SetValue(coord, i);
                            }
                        }
                        else
                        {
                            // Если конструктора нет, создадим объекты и попытаемся установить свойства X/Y
                            var coordDefaultCtor = coordsType.GetConstructor(Type.EmptyTypes);
                            var propX = coordsType.GetProperty("X") ?? coordsType.GetProperty("x");
                            var propY = coordsType.GetProperty("Y") ?? coordsType.GetProperty("y");
                            if (coordDefaultCtor == null || propX == null || propY == null)
                            {
                                // не можем заполнить Coordinates
                                message = "Found Replace(Coordinates[]) but cannot construct Coordinates instances (no ctor or X/Y props).";
                                // продолжим к следующему варианту
                            }
                            else
                            {
                                for (int i = 0; i < xs.Length; i++)
                                {
                                    var coord = coordDefaultCtor.Invoke(null);
                                    propX.SetValue(coord, xs[i]);
                                    propY.SetValue(coord, ys[i]);
                                    coordsArr.SetValue(coord, i);
                                }
                            }
                        }

                        // invoke
                        replaceCoords.Invoke(scatter, new object[] { coordsArr });
                        message = "Called public Replace(Coordinates[]).";
                        return true;
                    }
                }

                // 3) Попробуем найти непубличный сеттер свойства Data (Data имеет getter, но может иметь non-public setter)
                var propData = t.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (propData != null)
                {
                    var setMethod = propData.GetSetMethod(true);
                    if (setMethod != null)
                    {
                        // нужно определить ожидаемый тип (обычно Coordinates[] или IScatterSource)
                        var paramType = setMethod.GetParameters()[0].ParameterType;
                        // если принимает Coordinates[] — подготовим массив как выше
                        if (coordsType != null && paramType.IsArray && paramType.GetElementType() == coordsType)
                        {
                            var coordsArr = Array.CreateInstance(coordsType, xs.Length);
                            var ctor = coordsType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                 .FirstOrDefault(c =>
                                                 {
                                                     var p = c.GetParameters();
                                                     return p.Length == 2 && p[0].ParameterType == typeof(double) && p[1].ParameterType == typeof(double);
                                                 });
                            if (ctor != null)
                            {
                                for (int i = 0; i < xs.Length; i++)
                                    coordsArr.SetValue(ctor.Invoke(new object[] { xs[i], ys[i] }), i);
                            }
                            else
                            {
                                var coordDefaultCtor = coordsType.GetConstructor(Type.EmptyTypes);
                                var propX = coordsType.GetProperty("X") ?? coordsType.GetProperty("x");
                                var propY = coordsType.GetProperty("Y") ?? coordsType.GetProperty("y");
                                if (coordDefaultCtor != null && propX != null && propY != null)
                                {
                                    for (int i = 0; i < xs.Length; i++)
                                    {
                                        var coord = coordDefaultCtor.Invoke(null);
                                        propX.SetValue(coord, xs[i]);
                                        propY.SetValue(coord, ys[i]);
                                        coordsArr.SetValue(coord, i);
                                    }
                                }
                            }

                            setMethod.Invoke(scatter, new object[] { coordsArr });
                            message = "Set non-public Data via setter with Coordinates[].";
                            return true;
                        }

                        // если принимает two arrays (double[], double[])
                        if (paramType.IsArray && paramType.GetElementType() == typeof(double))
                        {
                            // нечасто, но попробуем
                            setMethod.Invoke(scatter, new object[] { xs });
                            // надо ещё выяснить, как передать ys — сложный случай, пропустим
                            message = "Called non-public set_Data(double[]?) - partial attempt.";
                            return true;
                        }
                    }
                }

                // 4) Попытка найти приватное поле, содержащее данные (например _data, data, _coordinates)
                string[] candidateFieldNames = new[] { "_data", "data", "_coordinates", "_coords", "coordinates" };
                foreach (var fname in candidateFieldNames)
                {
                    var fld = t.GetField(fname, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (fld != null)
                    {
                        // если поле типа Coordinates[]
                        var ftype = fld.FieldType;
                        if (coordsType != null && ftype.IsArray && ftype.GetElementType() == coordsType)
                        {
                            var coordsArr = Array.CreateInstance(coordsType, xs.Length);
                            var ctor = coordsType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                 .FirstOrDefault(c =>
                                                 {
                                                     var p = c.GetParameters();
                                                     return p.Length == 2 && p[0].ParameterType == typeof(double) && p[1].ParameterType == typeof(double);
                                                 });
                            if (ctor != null)
                            {
                                for (int i = 0; i < xs.Length; i++)
                                    coordsArr.SetValue(ctor.Invoke(new object[] { xs[i], ys[i] }), i);
                            }
                            else
                            {
                                var coordDefaultCtor = coordsType.GetConstructor(Type.EmptyTypes);
                                var propX = coordsType.GetProperty("X") ?? coordsType.GetProperty("x");
                                var propY = coordsType.GetProperty("Y") ?? coordsType.GetProperty("y");
                                if (coordDefaultCtor != null && propX != null && propY != null)
                                {
                                    for (int i = 0; i < xs.Length; i++)
                                    {
                                        var coord = coordDefaultCtor.Invoke(null);
                                        propX.SetValue(coord, xs[i]);
                                        propY.SetValue(coord, ys[i]);
                                        coordsArr.SetValue(coord, i);
                                    }
                                }
                            }

                            fld.SetValue(scatter, coordsArr);
                            message = $"Set private field '{fname}' with Coordinates[].";
                            return true;
                        }

                        // если поле типа (double[]) или tuple[]
                        if (ftype.IsArray && ftype.GetElementType() == typeof(double))
                        {
                            // например может быть поле Xs or Ys; попробуем найти matching fields for Xs and Ys
                            // best-effort: try to find sibling field for Ys/Xs
                            var otherName = fname.Contains("x", StringComparison.OrdinalIgnoreCase) ? fname.Replace("x", "y") : fname + "Ys";
                            var fld2 = t.GetField(otherName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            try
                            {
                                fld.SetValue(scatter, xs);
                                if (fld2 != null) fld2.SetValue(scatter, ys);
                                message = $"Set private fields '{fname}' and '{otherName}' with double[] if existed.";
                                return true;
                            }
                            catch { /* ignore and continue */ }
                        }
                    }
                }

                // 5) fallback: пересоздать scatter на UI-потоке (plotForFallback должен быть передан)
                if (plotForFallback != null)
                {
                    // Удаляем старый scatter и создаём новый с новыми массивами.
                    // Это делается на UI-потоке — вызывающий код должен это сделать через Dispatcher.
                    var plotType = plotForFallback.GetType();
                    // Удалим текущий scatter из plot
                    plotForFallback.Remove(scatter);
                    var newScatter = plotForFallback.Add.Scatter(xs, ys);
                    // если нужно, можно скопировать стиль через reflection (Color, LineWidth, MarkerSize ...)
                    try
                    {
                        // попытка копировать простые свойства, если они есть
                        CopyStyleIfPresent(scatter, newScatter);
                    }
                    catch { /* ignore */ }

                    message = "Fallback: removed old scatter and added a new one to plot.";
                    return true;
                }

                message = "No suitable method found to update Scatter; no plotForFallback provided.";
                return false;
            }
            catch (Exception ex)
            {
                message = "Exception during TryUpdateScatterData: " + ex.Message;
                return false;
            }
        }

        static void CopyStyleIfPresent(object from, object to)
        {
            if (from == null || to == null) return;
            var tFrom = from.GetType();
            var tTo = to.GetType();
            var props = new[] { "Color", "LineWidth", "MarkerSize", "MarkerShape" };
            foreach (var name in props)
            {
                var pFrom = tFrom.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var pTo = tTo.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (pFrom != null && pTo != null && pFrom.CanRead && pTo.CanWrite)
                {
                    try { pTo.SetValue(to, pFrom.GetValue(from)); } catch { }
                }
            }
        }
    }
}
