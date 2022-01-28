using System;
using System.Collections.Generic;
using Stratis.SmartContracts;

namespace Logicing.Knowing.UnitTests.Fakes
{
    public class PersistantStateFake : IPersistentState
    {
        private readonly IDictionary<string, object> _data;

        public PersistantStateFake()
        {
            _data = new Dictionary<string, object>();
        }

        public bool IsContract(Address address)
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytes(byte[] key)
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytes(string key)
        {
            throw new NotImplementedException();
        }

        public char GetChar(string key)
        {
            throw new NotImplementedException();
        }

        public Address GetAddress(string key)
        {
            if (_data.ContainsKey(key)) return (Address)_data[key];
            return Address.Zero;
        }

        public bool GetBool(string key)
        {
            return (bool)_data[key];
        }

        public int GetInt32(string key)
        {
            return (int)_data[key];
        }

        public uint GetUInt32(string key)
        {
            return (uint)_data[key];
        }

        public long GetInt64(string key)
        {
            throw new NotImplementedException();
        }

        public ulong GetUInt64(string key)
        {
            return (ulong)_data[key];
        }

        public UInt128 GetUInt128(string key)
        {
            throw new NotImplementedException();
        }

        public UInt256 GetUInt256(string key)
        {
            throw new NotImplementedException();
        }

        public string GetString(string key)
        {
            return (string)_data[key];
        }

        T IPersistentState.GetStruct<T>(string key)
        {
            if (_data.ContainsKey(key)) return (T)_data[key];
            return default;
        }

        public T[] GetArray<T>(string key)
        {
            if (_data.ContainsKey(key)) return (T[])_data[key];
            return new T[0];
        }

        public void SetBytes(byte[] key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void SetBytes(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void SetChar(string key, char value)
        {
            throw new NotImplementedException();
        }

        public void SetAddress(string key, Address value)
        {
            if (_data.ContainsKey(key)) _data[key] = value;
            else _data.Add(key, value);
        }

        public void SetBool(string key, bool value)
        {
            if (_data.ContainsKey(key)) _data[key] = value;
            else _data.Add(key, value);
        }

        public void SetInt32(string key, int value)
        {
            if (_data.ContainsKey(key)) _data[key] = value;
            else _data.Add(key, value);
        }

        public void SetUInt32(string key, uint value)
        {
            if (_data.ContainsKey(key)) _data[key] = value;
            else _data.Add(key, value);
        }

        public void SetInt64(string key, long value)
        {
            throw new NotImplementedException();
        }

        public void SetUInt64(string key, ulong value)
        {
            if (_data.ContainsKey(key)) _data[key] = value;
            else _data.Add(key, value);
        }

        public void SetUInt128(string key, UInt128 value)
        {
            throw new NotImplementedException();
        }

        public void SetUInt256(string key, UInt256 value)
        {
            throw new NotImplementedException();
        }

        public void SetString(string key, string value)
        {
            if (_data.ContainsKey(key)) _data[key] = value;
            else _data.Add(key, value);
        }

        void IPersistentState.SetStruct<T>(string key, T value)
        {
            if (_data.ContainsKey(key)) _data[key] = value;
            else _data.Add(new KeyValuePair<string, object>(key, value));
        }

        public void SetArray(string key, Array a)
        {
            if (_data.ContainsKey(key)) _data[key] = a;
            else _data.Add(key, a);
        }

        public void Clear(string key)
        {
            throw new NotImplementedException();
        }
    }
}