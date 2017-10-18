[assembly: System.Runtime.InteropServices.Guid("7e42fdd5-d7c4-4da7-b9a2-e2c8b11ad93a")]
[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.Reflection.AssemblyTitle("Телефония Билайн")]
[assembly: System.Reflection.AssemblyDescription("Провайдер для работы с сервером Билайн.")]
[assembly: EleWise.ELMA.ComponentModel.ComponentAssembly()]
[assembly: EleWise.ELMA.Model.Attributes.ModelAssembly()]

namespace Maxim5579.ELMA.Beeline_net452
{
    using System;


    [global::EleWise.ELMA.Model.Attributes.MetadataType(typeof(global::EleWise.ELMA.Model.Metadata.AssemblyInfoMetadata))]
    [global::EleWise.ELMA.Model.Attributes.Uid("7e42fdd5-d7c4-4da7-b9a2-e2c8b11ad93a")]
    [global::EleWise.ELMA.Model.Attributes.MetadataAccessLevel(global::EleWise.ELMA.Model.Metadata.MetadataAccessLevel.EleWise)]
    internal class @__AssemblyInfo
    {

        /// <summary>
        /// Уникальный идентификатор данного класса
        /// </summary>
        public const string UID_S = "7e42fdd5-d7c4-4da7-b9a2-e2c8b11ad93a";

        private static global::System.Guid _UID = new global::System.Guid(UID_S);

        /// <summary>
        /// Уникальный идентификатор данного класса
        /// </summary>
        public static global::System.Guid UID
        {
            get
            {
                return _UID;
            }
        }
    }

    internal class @__Resources__AssemblyInfo
    {

        public static string DisplayName
        {
            get
            {
                return global::EleWise.ELMA.SR.T("Телефония Билайн");
            }
        }

        public static string Description
        {
            get
            {
                return global::EleWise.ELMA.SR.T("Провайдер для работы с сервером Билайн.");
            }
        }
    }
}
