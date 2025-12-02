using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Generators
{
    public static class LayoutGeneratorFactory
    {
        public static Func<int, ILayoutGenerator>? Override { get; set; }

        public static ILayoutGenerator GetGenerator(int version)
        {
            if (Override != null)
                return Override(version);

            return version switch
            {
                1 => new LayoutV1Generator(),
                2 => new LayoutV2Generator(),
                _ => throw new System.ArgumentException($"Versão desconhecida: {version}")
            };
        }
    }

}

