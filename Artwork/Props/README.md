# Prop material artwork

AI-assisted higher-detail phone and bobblehead color atlases, based on their recovered UV layouts. Original generated files are preserved here; 1024px copies ship in the app. They are artistic reconstructions, not exact original pixels. Other props retain their original albedo with individual physically based finishes.

`Recovery/realism/prop-materials.json` defines the complete prop material pass. Phone screens use UV masks and a subtle emissive layer; bright neutral trim receives metalness. Pendulum/ballista atlas regions separate wood from metal. These masks approximate the authored atlas, not a scanned material segmentation. Glass is an alpha-blended PBR approximation without volumetric refraction or caustics.
