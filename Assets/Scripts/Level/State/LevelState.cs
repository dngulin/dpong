// This file is auto-generated by the PlainBuffers compiler
// Generated at 2021-03-14T19:09:12.1421920+03:00

// ReSharper disable All

using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using FxNet.Math;
using FxNet.Random;

#pragma warning disable 649

namespace DPong.Level.State {
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct HitPointsState {
        public const int SizeOf = 8;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public int Left;
        [FieldOffset(4)] public int Right;

        public void WriteDefault() {
            Left = 0;
            Right = 0;
        }

        public static bool operator ==(in HitPointsState l, in HitPointsState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in HitPointsState l, in HitPointsState r) => !(l == r);

        public override bool Equals(object obj) => obj is HitPointsState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct BallState {
        public const int SizeOf = 40;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public FxNum FreezeCooldown;
        [FieldOffset(8)] public FxVec2 Speed;
        [FieldOffset(24)] public FxVec2 Position;

        public void WriteDefault() {
            FreezeCooldown = default;
            Speed = FxVec2.Zero;
            Position = FxVec2.Zero;
        }

        public static bool operator ==(in BallState l, in BallState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in BallState l, in BallState r) => !(l == r);

        public override bool Equals(object obj) => obj is BallState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct BlockerState {
        public const int SizeOf = 16;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public FxVec2 Position;

        public void WriteDefault() {
            Position = FxVec2.Zero;
        }

        public static bool operator ==(in BlockerState l, in BlockerState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in BlockerState l, in BlockerState r) => !(l == r);

        public override bool Equals(object obj) => obj is BlockerState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public unsafe struct BlockerState2 {
        public const int SizeOf = 32;
        public const int Length = 2;

        private fixed byte _buffer[SizeOf];

        public void WriteDefault() {
            for (var i = 0; i < Length; i++) {
                this[i].WriteDefault();
            }
        }

        public ref BlockerState this[int index] {
            get {
                if (index < 0 || sizeof(BlockerState) * index >= SizeOf) throw new IndexOutOfRangeException();
                return ref At(index);
            }
        }

        private ref BlockerState At(int index) {
            fixed (byte* __ptr = _buffer) {
                return ref *((BlockerState*)__ptr + index);
            }
        }

        public _EnumeratorOfBlockerState2 GetEnumerator() => new _EnumeratorOfBlockerState2(ref this);

        public unsafe ref struct _EnumeratorOfBlockerState2 {
            private readonly BlockerState2* _arrayPtr;
            private int _index;

            public _EnumeratorOfBlockerState2(ref BlockerState2 array) {
                fixed (BlockerState2* arrayPtr = &array) _arrayPtr = arrayPtr;
                _index = -1;
            }

            public bool MoveNext() => ++_index < Length;
            public ref BlockerState Current => ref (*_arrayPtr).At(_index);

            public void Reset() => _index = -1;
            public void Dispose() {}
        }

        public static bool operator ==(in BlockerState2 l, in BlockerState2 r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in BlockerState2 l, in BlockerState2 r) => !(l == r);

        public override bool Equals(object obj) => obj is BlockerState2 casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct LevelState {
        public const int SizeOf = 120;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public FxNum Pace;
        [FieldOffset(8)] public FxRandomState Random;
        [FieldOffset(40)] public BallState Ball;
        [FieldOffset(80)] public BlockerState2 Blockers;
        [FieldOffset(112)] public HitPointsState HitPoints;

        public void WriteDefault() {
            Pace = default;
            Random = default;
            Ball.WriteDefault();
            Blockers.WriteDefault();
            HitPoints.WriteDefault();
        }

        public static bool operator ==(in LevelState l, in LevelState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in LevelState l, in LevelState r) => !(l == r);

        public override bool Equals(object obj) => obj is LevelState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }
}
