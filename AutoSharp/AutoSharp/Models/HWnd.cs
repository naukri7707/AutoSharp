using System;

namespace AutoSharp.Models
{
    /// <summary>
    /// The window handle.
    /// </summary>
    public readonly partial struct HWnd : IEquatable<HWnd>
    {
        public HWnd(IntPtr hWnd)
        {
            value = hWnd;
        }

        private readonly IntPtr value;

        /// <summary>
        /// Get whether this <see cref="HWnd"/> is null or not.
        /// </summary>
        public bool IsNull => value == default;

        public static explicit operator HWnd(IntPtr value)
        {
            return new HWnd(value);
        }

        public static implicit operator IntPtr(HWnd value)
        {
            return value.value;
        }

        public static bool operator !=(HWnd left, HWnd right)
        {
            return !(left == right);
        }

        public static bool operator ==(HWnd left, HWnd right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is HWnd hWnd)
                return Equals(hWnd);
            return false;
        }

        public bool Equals(HWnd other)
        {
            return value == other.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    partial struct HWnd
    {
        /// <summary>
        /// The null window handle.<para />
        /// This is often indicates all of the windows.
        /// </summary>
        public static HWnd Null => default;
    }
}
