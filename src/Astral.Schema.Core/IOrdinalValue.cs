using System;
using System.Collections.Generic;

namespace Astral.Schema.Core
{
    public interface IDataValue
    {
    
    }
    
    public interface IBasicValue : IDataValue
    {
        object Value { get; }
        Type Type { get; }
        bool CanConvert(Type type);
        bool TryConvert(Type type, out object value);
    }

    public interface IOrdinalValue : IBasicValue
    {
        
    }

    public interface IArrayValue : IDataValue
    {
        int Count { get; }
        IDataValue this[int index] { get; }
        IArrayValue SetItem(int index, IDataValue value);
        IArrayValue Add(IDataValue value);
        IArrayValue RemoveAt(int index);
        IArrayValue Remove(IDataValue value);
        IArrayValue Insert(int index, IDataValue value);
        IArrayValue Replace(IDataValue oldValue, IDataValue newValue);
        int IndexOf(IDataValue value);
        bool Contains(IDataValue value);
    }

    public interface IMapValueElement 
    {
        string Name { get; }
        IDataValue Value { get; }
    }

    public interface IMapValue : IDataValue
    {
        IMapValue Add(IMapValueElement element);
        IMapValue Add(string name, IDataValue value);
        IDataValue this[string name] { get; }
        int Count { get; }
        IMapValueElement this[int index] { get; }
        IEnumerable<IDataValue> GetAll(string name);
        IMapValue SetItem(string name, IDataValue value);
        IMapValue RemoveItem(string name);
    }

    public interface IDataType
    {
        
    }

    public interface IBasicType : IDataType
    {
        
    }
    
    public interface IOrdinalType : IBasicType
    {
        
    }

    public interface IArrayType : IDataType
    {
        IDataType ElementType { get; }
    }

    public interface IMayBeType : IDataType
    {
        IDataType ElementType { get; }
    }

    public interface IEnumField
    {
        
    }
    
    

    public interface ISchema
    {
        IOrdinalValue CreateValue(byte value);
        IOrdinalValue CreateValue(sbyte value);
        IOrdinalValue CreateValue(ushort value);
        IOrdinalValue CreateValue(short value);
        IOrdinalValue CreateValue(uint value);
        IOrdinalValue CreateValue(int value);
        IOrdinalValue CreateValue(ulong value);
        IOrdinalValue CreateValue(long value);
        IOrdinalValue CreateValue(string value);
        IOrdinalValue CreateValue(Guid value);
        IOrdinalValue CreateValue(DateTime value);
        IOrdinalValue CreateValue(DateTimeOffset value);
        IOrdinalValue CreateValue(TimeSpan value);
        IArrayValue CreateArray(IEnumerable<IDataValue> values);
        IArrayValue CreateArray(params IDataValue[] values);
        IMapValueElement CreateMapValueElement(string name, IDataValue value);
        IMapValue CreateMapValue(IEnumerable<KeyValuePair<string, IDataValue>> pairs);
        IMapValue CreateMapValue(IEnumerable<IMapValueElement> elements);
        IMapValue CreateMapValue(params (string, IDataValue)[] values);
    }
}