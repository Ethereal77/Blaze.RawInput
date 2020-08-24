// Copyright © 2020 Infinisis

using System;
using System.Collections.Generic;

namespace Blaze.Collections
{
  internal static class EqualityComparer
  {
    public static readonly IEqualityComparer<IntPtr> DefaultIntPtr = new IntPtrComparer();

    internal class IntPtrComparer : EqualityComparer<IntPtr>
    {
      public override bool Equals(IntPtr x, IntPtr y) => x == y;

      public override int GetHashCode(IntPtr obj) => obj.GetHashCode();
    }
  }
}
