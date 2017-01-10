# Generators
All the generatos inherit from Generator.cs and a list of ChunkData.

You only need to override the Generate function to generate your own data.
`public virtual ChunkData Generate(WorldPos pos)`
