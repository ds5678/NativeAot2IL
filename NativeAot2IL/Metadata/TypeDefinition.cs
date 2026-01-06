using System.Reflection;

namespace NativeAot2IL.Metadata;

public class TypeDefinition : ReadableClass
{
    public TypeAttributes Flags { get; private set; }

    /// One of: TypeDefinition, TypeReference, TypeSpecification
    public MetadataHandle BaseType { get; private set; }
    public MetadataHandle NamespaceDefinition { get; private set; }

    private MetadataHandle _nameHandle;
    public string? Name { get; private set; }
    
    public uint Size { get; private set; }
    public ushort PackingSize { get; private set; }
    public MetadataHandle EnclosingType { get; private set; }
    
    public MetadataHandle[] NestedTypeHandles { get; private set; }
    public List<TypeDefinition> NestedTypes { get; } = new();
    
    public MetadataHandle[] MethodHandles { get; private set; }
    public List<MethodDefinition> Methods { get; } = new();
    
    public MetadataHandle[] FieldHandles { get; private set; }
    public List<FieldDefinition> Fields { get; } = new();
    
    public MetadataHandle[] PropertyHandles { get; private set; }
    public List<PropertyDefinition> Properties { get; } = new();
    
    public MetadataHandle[] EventHandles { get; private set; }
    public List<EventDefinition> Events { get; } = new();
    
    public MetadataHandle[] GenericParameters { get; private set; }

    /// One of: TypeDefinition, TypeReference, TypeSpecification
    public MetadataHandle[] Interfaces { get; private set; }
    public MetadataHandle[] CustomAttributes { get; private set; }
    
    public override void Read(ClassReadingBinaryReader reader)
    {
        Flags = (TypeAttributes)reader.ReadCompressedUIntHereNoLock();
        BaseType = reader.ReadMetadataHandleHereNoLock(HandleType.Null); //One of three possible types so we pass null and it reads differently
        NamespaceDefinition = reader.ReadMetadataHandleHereNoLock(HandleType.NamespaceDefinition);
        
        _nameHandle = reader.ReadMetadataHandleHereNoLock(HandleType.ConstantStringValue);
        
        Size = reader.ReadCompressedUIntHereNoLock();
        PackingSize = (ushort)reader.ReadCompressedUIntHereNoLock();
        EnclosingType = reader.ReadMetadataHandleHereNoLock(HandleType.TypeDefinition);
        NestedTypeHandles = reader.ReadMetadataHandleArrayHereNoLock(HandleType.TypeDefinition);
        MethodHandles = reader.ReadMetadataHandleArrayHereNoLock(HandleType.Method);
        FieldHandles = reader.ReadMetadataHandleArrayHereNoLock(HandleType.Field);
        PropertyHandles = reader.ReadMetadataHandleArrayHereNoLock(HandleType.Property);
        EventHandles = reader.ReadMetadataHandleArrayHereNoLock(HandleType.Event);
        GenericParameters = reader.ReadMetadataHandleArrayHereNoLock(HandleType.GenericParameter);
        Interfaces = reader.ReadMetadataHandleArrayHereNoLock(HandleType.Null); //One of three possible types so we pass null and it reads differently
        CustomAttributes = reader.ReadMetadataHandleArrayHereNoLock(HandleType.CustomAttribute);
        
        Name = _nameHandle.ResolveString(reader, false);
        
        NestedTypes.EnsureCapacity(NestedTypeHandles.Length);
        foreach (var nestedTypeHandle in NestedTypeHandles)
        {
            NestedTypes.Add(nestedTypeHandle.Resolve<TypeDefinition>(reader, false) ?? throw new InvalidOperationException($"Failed to resolve TypeDefinition for handle {nestedTypeHandle}"));
        }
        
        Methods.EnsureCapacity(MethodHandles.Length);
        foreach (var methodHandle in MethodHandles)
        {
            Methods.Add(methodHandle.Resolve<MethodDefinition>(reader, false) ?? throw new InvalidOperationException($"Failed to resolve MethodDefinition for handle {methodHandle}"));
        }
        
        Fields.EnsureCapacity(FieldHandles.Length);
        foreach (var fieldHandle in FieldHandles)
        {
            Fields.Add(fieldHandle.Resolve<FieldDefinition>(reader, false) ?? throw new InvalidOperationException($"Failed to resolve FieldDefinition for handle {fieldHandle}"));
        }
        
        Properties.EnsureCapacity(PropertyHandles.Length);
        foreach (var propertyHandle in PropertyHandles)
        {
            Properties.Add(propertyHandle.Resolve<PropertyDefinition>(reader, false) ?? throw new InvalidOperationException($"Failed to resolve PropertyDefinition for handle {propertyHandle}"));
        }
        
        Events.EnsureCapacity(EventHandles.Length);
        foreach (var eventHandle in EventHandles)
        {
            Events.Add(eventHandle.Resolve<EventDefinition>(reader, false) ?? throw new InvalidOperationException($"Failed to resolve EventDefinition for handle {eventHandle}"));
        }
    }

    public override string ToString()
    {
        return $"TypeDefinition: {Name}, Methods={Methods.Count}, Fields={Fields.Count}, Properties={Properties.Count}, Events={EventHandles.Length}, NestedTypes={NestedTypeHandles.Length}, Flags={Flags}";
    }
}