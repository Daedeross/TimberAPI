using UnityEngine;

namespace TimberApi.SpecificationSystem.SpecificationTypes
{
    internal class TimberbornSpecification : ISpecification
    {
        public TimberbornSpecification(TextAsset specificationAsset)
        {
            string name = specificationAsset.name;
            FullName = name.ToLower();
            SpecificationAsset = specificationAsset;
            SpecificationName = name[..name.IndexOf('.')].ToLower();
            IsOriginal = true;
        }

        private TextAsset SpecificationAsset { get; }

        public string FullName { get; }

        public string SpecificationName { get; }

        public bool IsOriginal { get; }

        public bool IsReplace { get; }

        public string LoadJson()
        {
            return SpecificationAsset.text;
        }
    }
}