// Copyright © 2020 Infinisis

using System;
using System.Runtime.InteropServices;

namespace Blaze.Interop.Win32
{
    /// <summary>
    ///   A boolean value stored using 4 bytes.
    /// </summary>
    /// <remarks>
    ///   In .NET, the <see cref="bool"/> type is a boolean value stored in a single byte.
    ///   This type <see cref="Bool"/>, however, is stored using a DWORD (4-byte integer) in
    ///   the same way as the <c>BOOL</c> Win32 API type. It is used primarily for interop
    ///   with de Windows APIs and related libraries.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly struct Bool : IEquatable<Bool>
    {
        private readonly int boolValue;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Bool"/> structure.
        /// </summary>
        /// <param name="boolValue">Value to encapsulate in this bool structure.</param>
        public Bool(bool value)
        {
            boolValue = value ? 1 : 0;
        }

        /// <summary>
        ///   Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><c>true</c> if <paramref name="other" /> and this instance represent the same value; otherwise, <c>false</c>.</returns>
        public bool Equals(Bool other) => boolValue == other.boolValue;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return obj is Bool @bool && Equals(@bool);
        }

        public override int GetHashCode() => boolValue;

        public static bool operator ==(Bool left, Bool right) => left.Equals(right);

        public static bool operator !=(Bool left, Bool right) => !left.Equals(right);

        /// <summary>
        ///   Performs an implicit conversion from <see cref="Bool"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="booleanValue">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator bool(Bool booleanValue) => booleanValue.boolValue != 0;

        /// <summary>
        ///   Performs an implicit conversion from <see cref="bool"/> to <see cref="Bool"/>.
        /// </summary>
        /// <param name="boolValue">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Bool(bool boolValue) => new Bool(boolValue);

        public override string ToString() => (boolValue != 0).ToString();
    }
}
