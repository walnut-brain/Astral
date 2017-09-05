using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Astral.Payloads.DataContracts
{
    public class WellKnownTypes
    {
        private readonly IReadOnlyDictionary<Type, string> _typeIndex;
        private readonly IReadOnlyDictionary<string, Type> _codeIndex;
        private readonly IReadOnlyCollection<Type> _unitTypes;
        
        public WellKnownTypes(Type defaultUnitType,
            IEnumerable<Type> additionalUnitTypes, IEnumerable<WellKnownTypeDescriptor> descriptors)
        {
            if (defaultUnitType == null) throw new ArgumentNullException(nameof(defaultUnitType));

            additionalUnitTypes = additionalUnitTypes ?? Enumerable.Empty<Type>();
            descriptors = descriptors ?? Enumerable.Empty<WellKnownTypeDescriptor>();
            Types = new ReadOnlyCollection<WellKnownTypeDescriptor>(descriptors.ToList());
            _typeIndex = Types.ToDictionary(p => p.Type, p => p.Code);
            _codeIndex = Types.ToDictionary(p => p.Code, p => p.Type);
            _unitTypes = new ReadOnlyCollection<Type>(new [] {defaultUnitType} .Union(additionalUnitTypes).Distinct().ToList());
            
        }

        public bool TryGetCode(Type type, out string code) => _typeIndex.TryGetValue(type, out code);


        public bool TryGetType(string code, out Type type) => _codeIndex.TryGetValue(code, out type);
        
        public IReadOnlyCollection<WellKnownTypeDescriptor> Types { get; }

        public bool IsUnit(Type type) => _unitTypes.Contains(type);
        

        public bool IsUnit(string code) => code == UnitCode;
        

        public static string UnitCode = "unit";
        public Type DefaultUnitType => _unitTypes.First();

        public WellKnownTypes SetDefaultUnitType(Type unitType) 
            => DefaultUnitType == unitType ? this : new WellKnownTypes(unitType, _unitTypes, Types);

        public WellKnownTypes AddUnitType(Type unitType) 
            => IsUnit(unitType) ? this : new WellKnownTypes(DefaultUnitType, _unitTypes.Union(new[] {unitType}), Types);

        public WellKnownTypes AddDescriptor(WellKnownTypeDescriptor descriptor)
            => new WellKnownTypes(DefaultUnitType, _unitTypes, Types.Union(new [] {descriptor}));
            
        public static WellKnownTypes Default = new WellKnownTypes(typeof(ValueTuple), null, new []
        {
            new WellKnownTypeDescriptor(typeof(bool), "bool", "boolean"),
            new WellKnownTypeDescriptor(typeof(byte), "u8", "byte - unsigned 8-bit whole number"),
            new WellKnownTypeDescriptor(typeof(sbyte), "i8", "signed byte - signed 8-bit whole number)"),
            new WellKnownTypeDescriptor(typeof(ushort), "u16", "unsigned 16-bit whole number"),
            new WellKnownTypeDescriptor(typeof(short), "i16", "signed 16-bit whole number"),
            new WellKnownTypeDescriptor(typeof(int), "i32", "signed 32-bit whole number"),
            new WellKnownTypeDescriptor(typeof(uint), "u32", "unsigned 32-bit whole number"),
            new WellKnownTypeDescriptor(typeof(long), "i64", "signed 64-bit whole number"),
            new WellKnownTypeDescriptor(typeof(ulong), "u64", "unsigned 64-bit whole number"),
            new WellKnownTypeDescriptor(typeof(double), "f64", "64-bit floating point number"),
            new WellKnownTypeDescriptor(typeof(float), "f32", "32-bit floating point number"), 
            new WellKnownTypeDescriptor(typeof(string), "string", "string")
        });
    }
}