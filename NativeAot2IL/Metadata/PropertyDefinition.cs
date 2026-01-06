using System.Reflection;

namespace NativeAot2IL.Metadata;

public class PropertyDefinition : ReadableClass
{
    public PropertyAttributes Flags { get; private set; }
    private MetadataHandle _nameHandle; //ConstantStringValue
    public string? Name { get; private set; }
    public MetadataHandle Signature { get; private set; } //PropertySignature
    public MetadataHandle[] MethodSemantics { get; private set; } //MethodSemantics
    
    /// One of: TypeDefinition, TypeReference, TypeSpecification, ConstantBooleanArray, ConstantBooleanValue, ConstantByteArray, ConstantByteValue, ConstantCharArray, ConstantCharValue, ConstantDoubleArray, ConstantDoubleValue, ConstantEnumArray, ConstantEnumValue, ConstantHandleArray, ConstantInt16Array, ConstantInt16Value, ConstantInt32Array, ConstantInt32Value, ConstantInt64Array, ConstantInt64Value, ConstantReferenceValue, ConstantSByteArray, ConstantSByteValue, ConstantSingleArray, ConstantSingleValue, ConstantStringArray, ConstantStringValue, ConstantUInt16Array, ConstantUInt16Value, ConstantUInt32Array, ConstantUInt32Value, ConstantUInt64Array, ConstantUInt64Value
    public MetadataHandle DefaultValue { get; private set; }
    
    public MetadataHandle[] CustomAttributes { get; private set; } //CustomAttribute
    
    public override void Read(ClassReadingBinaryReader reader)
    {
        Flags = (PropertyAttributes)reader.ReadCompressedUIntHereNoLock();
        _nameHandle = reader.ReadMetadataHandleHereNoLock(HandleType.ConstantStringValue);
        Signature = reader.ReadMetadataHandleHereNoLock(HandleType.PropertySignature);
        MethodSemantics = reader.ReadMetadataHandleArrayHereNoLock(HandleType.MethodSemantics);
        DefaultValue = reader.ReadMetadataHandleHereNoLock(HandleType.Null); //One of many possible types so we pass null and it reads differently
        CustomAttributes = reader.ReadMetadataHandleArrayHereNoLock(HandleType.CustomAttribute);
        
        Name = _nameHandle.ResolveString(reader, false);
    }
    
    public override string ToString()
    {
        return $"PropertyDefinition: {Name}, MethodSemantics={MethodSemantics.Length}, CustomAttributes={CustomAttributes.Length}, Flags={Flags}";
    }
}