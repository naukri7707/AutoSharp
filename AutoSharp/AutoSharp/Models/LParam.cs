namespace AutoSharp.Models
{
    /// <summary>
    /// The value of <see cref="WndProcMsg.lParam"/>
    /// </summary>
    public enum LParam : uint
    {
        KeyDown = 1,

        KeyUp = KeyDown | 1u << 30 | 1u << 31
    }
}
