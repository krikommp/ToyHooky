using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MyHoot
{
    public unsafe class Hook
    {
        public bool Enabled;
        private readonly MethodInfo _from;
        private readonly MethodInfo _to;
        private readonly MethodInfo _proxy;
        private readonly bool _is64Bit = IntPtr.Size == 8;
        private byte[] _originalMethod;
        private byte[] _tmpMethod_1;
        private byte[] _tmpMethod_2;

        public Hook(MethodInfo from, MethodInfo to, MethodInfo proxy)
        {
            this._from = from;
            this._to = to;
            this._proxy = proxy;
        }

        public void Enable()
        {
            this.Enabled = !this.Enabled ? true : throw new Exception("Method already hooked.");
            RuntimeHelpers.PrepareMethod(this._from.MethodHandle);
            RuntimeHelpers.PrepareMethod(this._to.MethodHandle);
            RuntimeHelpers.PrepareMethod(this._proxy.MethodHandle);
            IntPtr functionPointer1 = this._from.MethodHandle.GetFunctionPointer();
            IntPtr functionPointer2 = this._to.MethodHandle.GetFunctionPointer();
            IntPtr functionPointer3 = this._proxy.MethodHandle.GetFunctionPointer();
            if (this._is64Bit)
            {
                this._originalMethod = Memory.ReadBytes(functionPointer1, 13);
                _tmpMethod_1 = Memory.ReadBytes(functionPointer2, 13);
                _tmpMethod_2 = Memory.ReadBytes(functionPointer3, 13);

                Memory.WriteByte(functionPointer1, (byte)73);
                Memory.WriteByte(functionPointer1 + 1, (byte)187);
                Memory.WriteLong(functionPointer1 + 2, functionPointer2.ToInt64());
                Memory.WriteByte(functionPointer1 + 10, (byte)65);
                Memory.WriteByte(functionPointer1 + 11, byte.MaxValue);
                Memory.WriteByte(functionPointer1 + 12, (byte)227);

                Memory.WriteBytes(functionPointer3, this._originalMethod);
                long* jmpFrom = (long*)(functionPointer3.ToPointer()) + 13;
                long* jmpTo = (long*)(functionPointer1.ToPointer()) + 13;

                int val = (int)jmpTo - (int)jmpFrom - 5;
                byte* ptr = (byte*)jmpFrom;
                *ptr = 0xE9;
                int* pOffset = (int*)(ptr + 1);
                *pOffset = (int)val;
            }
            else
            {
                this._originalMethod = Memory.ReadBytes(functionPointer1, 6);
                Memory.WriteByte(functionPointer1, (byte)233);
                Memory.WriteInt(functionPointer1 + 1, functionPointer2.ToInt32() - functionPointer1.ToInt32() - 5);
                Memory.WriteByte(functionPointer1 + 5, (byte)195);
            }
        }

        public void Disable()
        {
            this.Enabled = this.Enabled ? false : throw new Exception("Method not hooked.");
            Memory.WriteBytes(this._from.MethodHandle.GetFunctionPointer(), this._originalMethod);
        }

        public T Call<T>(object instance, object[] parameters)
        {
            bool enabled = this.Enabled;
            if (enabled)
                this.Disable();
            object obj = this._from.Invoke(instance, parameters);
            if (enabled)
                this.Enable();
            return (T)obj;
        }

        public void Call(object instance, object[] parameters)
        {
            int num = this.Enabled ? 1 : 0;
            if (num != 0)
                this.Disable();
            this._from.Invoke(instance, parameters);
            if (num == 0)
                return;
            this.Enable();
        }
        
        public string PrintAddrs()
        {
            void* _pTarget = this._from.MethodHandle.GetFunctionPointer().ToPointer();
            void* _pReplace = this._to.MethodHandle.GetFunctionPointer().ToPointer();
            void* _pProxy = this._proxy.MethodHandle.GetFunctionPointer().ToPointer();
            if (IntPtr.Size == 4)
                return $"target:0x{(uint)_pTarget:x}, replace:0x{(uint)_pReplace:x}, proxy:0x{(uint)_pProxy:x}";
            else
                return $"target:0x{(ulong)_pTarget:x}, replace:0x{(ulong)_pReplace:x}, proxy:0x{(ulong)_pProxy:x}";
        }

        ~Hook()
        {
            if (!this.Enabled)
                return;
            Memory.WriteBytes(this._from.MethodHandle.GetFunctionPointer(), this._originalMethod);
        }
    }
}