using System.Reflection;

namespace NativeAot2IL.Metadata;

public class FieldDefinition : ReadableClass
{
    public FieldAttributes Flags { get; private set; }
    public string? Name { get; private set; }
    private MetadataHandle _nameHandle;  //ConstantStringValue
    public MetadataHandle Signature { get; private set; } //FieldSignature

    /// One of: TypeDefinition, TypeReference, TypeSpecification, ConstantBooleanArray, ConstantBooleanValue, ConstantByteArray, ConstantByteValue, ConstantCharArray, ConstantCharValue, ConstantDoubleArray, ConstantDoubleValue, ConstantEnumArray, ConstantEnumValue, ConstantHandleArray, ConstantInt16Array, ConstantInt16Value, ConstantInt32Array, ConstantInt32Value, ConstantInt64Array, ConstantInt64Value, ConstantReferenceValue, ConstantSByteArray, ConstantSByteValue, ConstantSingleArray, ConstantSingleValue, ConstantStringArray, ConstantStringValue, ConstantUInt16Array, ConstantUInt16Value, ConstantUInt32Array, ConstantUInt32Value, ConstantUInt64Array, ConstantUInt64Value
    public MetadataHandle DefaultValue { get; private set; }

    public uint Offset { get; private set; }
    public MetadataHandle[] CustomAttributes { get; private set; } //CustomAttribute
    
    public override void Read(ClassReadingBinaryReader reader)
    {
        Flags = (FieldAttributes)reader.ReadCompressedUIntHereNoLock();
        _nameHandle = reader.ReadMetadataHandleHereNoLock(HandleType.ConstantStringValue);
        Signature = reader.ReadMetadataHandleHereNoLock(HandleType.FieldSignature);
        DefaultValue = reader.ReadMetadataHandleHereNoLock(HandleType.Null); //One of many possible types so we pass null and it reads differently
        Offset = reader.ReadCompressedUIntHereNoLock();
        CustomAttributes = reader.ReadMetadataHandleArrayHereNoLock(HandleType.CustomAttribute);
        
        Name = _nameHandle.ResolveString(reader, false);
    }
    
    public override string ToString()
    {
        return $"FieldDefinition: {Name}, Offset={Offset}, CustomAttributes={CustomAttributes.Length}, Flags={Flags}";
    }
}