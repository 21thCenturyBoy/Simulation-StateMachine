using System.ComponentModel;
using System.Text.RegularExpressions;

namespace SSMTool
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 检查命名规范
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool RegexMatchName(this string name)
        {
            string expr = "^[a-zA-Z_][a-zA-Z0-9_]*$";
            return Regex.IsMatch(name, expr);
        }
        /// <summary>
        /// 检查命名规范
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string FixedName(this string name)
        {
            return "Fixed_"+name ;//拼接前缀
        }
    }
}
