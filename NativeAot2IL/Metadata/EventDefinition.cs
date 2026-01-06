using System.Reflection;

namespace NativeAot2IL.Metadata;

public class EventDefinition : ReadableClass
{
    public EventAttributes Flags { get; private set; }
    
    public string? Name { get; private set; } //ConstantStringValue
    private MetadataHandle _nameHandle;

    /// One of: TypeDefinition, TypeReference, TypeSpecification
    public MetadataHandle Type { get; private set; }

    public MetadataHandle[] MethodSemantics { get; private set; } //MethodSemantics
    public MetadataHandle[] CustomAttributes { get; private set; } //CustomAttribute
    
    public override void Read(ClassReadingBinaryReader reader)
    {
        Flags = (EventAttributes)reader.ReadCompressedUIntHereNoLock();
        _nameHandle = reader.ReadMetadataHandleHereNoLock(HandleType.ConstantStringValue);
        Type = reader.ReadMetadataHandleHereNoLock(HandleType.Null); //One of three possible types so we pass null and it reads differently
        MethodSemantics = reader.ReadMetadataHandleArrayHereNoLock(HandleType.MethodSemantics);
        CustomAttributes = reader.ReadMetadataHandleArrayHereNoLock(HandleType.CustomAttribute);

        Name = _nameHandle.ResolveString(reader, false);
    }
    
    public override string ToString()
    {
        return $"EventDefinition: {Name}, MethodSemantics={MethodSemantics.Length}, CustomAttributes={CustomAttributes.Length}, Flags={Flags}";
    }
}