using Chara;
using System;
using System.Collections.Generic;

namespace RGExtendedSave.Ext
{
    class ExtKeyContainer
    {
        public FaceKeys Face { get; set; } = new FaceKeys();
        public IntPtr Body { get; set; }
        public CoordinateKeys[] Coordinate { get; set; } = new CoordinateKeys[3];
        public IntPtr GameInfo { get; set; }
        public IntPtr FileParameter { get; set; }
        public IntPtr FileStatus { get; set; }

        public class FaceKeys
        {
            public IntPtr Face { get; set; }
            public IntPtr[] Eyes { get; set; } = new IntPtr[2];
            public IntPtr Makeup { get; set; }

            public void MoveKeys(ChaFileFace cff)
            {
                Extensions.MoveData(Face, cff.Pointer);

                for (int i = 0; i < (Eyes?.Length ?? 0); i++)
                {
                    Extensions.MoveData(Eyes[i], cff.pupil[i].Pointer);
                }

                Extensions.MoveData(Makeup, cff.makeup.Pointer);
            }

            public void RemoveKeys()
            {
                Extensions.Remove(Face);

                for (int i = 0; i < (Eyes?.Length ?? 0); i++)
                {
                    Extensions.Remove(Eyes[i]);
                }

                Extensions.Remove(Makeup);
            }
        }

        public class CoordinateKeys
        {
            public IntPtr Clothes { get; set; }
            public ClothesPartKeys[] ClothesParts { get; set; } = new ClothesPartKeys[8];
            public IntPtr Accessory { get; set; }
            public AccessoryPartKeys[] AccessoryParts { get; set; } = new AccessoryPartKeys[20];
            public IntPtr Hair { get; set; }
            public HairPartKeys[] HairParts { get; set; } = new HairPartKeys[4];

            public void MoveOutfitKeys(ChaFileCoordinate cfc)
            {
                Extensions.MoveData(Clothes, cfc.clothes.Pointer);

                for (int i = 0; i < (ClothesParts?.Length ?? 0); i++)
                {
                    ClothesParts[i]?.MoveKeys(cfc.clothes.parts[i]);
                }

                Extensions.MoveData(Accessory, cfc.accessory.Pointer);

                for (int i = 0; i < (AccessoryParts?.Length ?? 0); i++)
                {
                    AccessoryParts[i]?.MoveKeys(cfc.accessory.parts[i]);
                }
            }

            public void MoveHairKeys(ChaFileCoordinate cfc)
            {
                Extensions.MoveData(Hair, cfc.hair.Pointer);

                for (int i = 0; i < (HairParts?.Length ?? 0); i++)
                {
                    HairParts[i]?.MoveKeys(cfc.hair.parts[i]);
                }
            }

            public void RemoveOutfitKeys()
            {
                Extensions.Remove(Clothes);

                for (int i = 0; i < (ClothesParts?.Length ?? 0); i++)
                {
                    ClothesParts[i]?.RemoveKeys();
                }

                Extensions.Remove(Accessory);

                for (int i = 0; i < (AccessoryParts?.Length ?? 0); i++)
                {
                    AccessoryParts[i]?.RemoveKeys();
                }
            }

            public void RemoveHairKeys()
            {
                Extensions.Remove(Hair);

                for (int i = 0; i < (HairParts?.Length ?? 0); i++)
                {
                    HairParts[i]?.RemoveKeys();
                }
            }
        }

        public class ClothesPartKeys
        {
            public IntPtr Part { get; set; }
            public IntPtr[] PartColors { get; set; } = new IntPtr[3];

            public void MoveKeys(ChaFileClothes.PartsInfo to)
            {
                Extensions.MoveData(Part, to.Pointer);

                for (int i = 0; i < (PartColors?.Length ?? 0); i++)
                {
                    Extensions.MoveData(PartColors[i], to.colorInfo[i].Pointer);
                }
            }

            public void RemoveKeys()
            {
                Extensions.Remove(Part);

                for (int i = 0; i < (PartColors?.Length ?? 0); i++)
                {
                    Extensions.Remove(PartColors[i]);
                }
            }
        }

        public class AccessoryPartKeys
        {
            public IntPtr Part { get; set; }
            public IntPtr[] PartColors { get; set; } = new IntPtr[4];

            public void MoveKeys(ChaFileAccessory.PartsInfo to)
            {
                Extensions.MoveData(Part, to.Pointer);

                for (int i = 0; i < (PartColors?.Length ?? 0); i++)
                {
                    Extensions.MoveData(PartColors[i], to.colorInfo[i].Pointer);
                }
            }

            public void RemoveKeys()
            {
                Extensions.Remove(Part);

                for (int i = 0; i < (PartColors?.Length ?? 0); i++)
                {
                    Extensions.Remove(PartColors[i]);
                }
            }
        }

        public class HairPartKeys
        {
            public IntPtr Part { get; set; }
            public IntPtr[] PartColors { get; set; } = new IntPtr[4];
            public Dictionary<int, string> PartBundles { get; set; } = new Dictionary<int, string>();

            public void MoveKeys(ChaFileHair.PartsInfo to)
            {
                Extensions.MoveData(Part, to.Pointer);

                for (int i = 0; i < (PartColors?.Length ?? 0); i++)
                {
                    Extensions.MoveData(PartColors[i], to.acsColorInfo[i].Pointer);
                }

                foreach (KeyValuePair<int, string> kv in PartBundles)
                {
                    if (to.dictBundle?[kv.Key] != null)
                    {
                        Extensions.MoveData($"{Part}-{kv.Key}", $"{to.Pointer}-{kv.Key}");
                    }
                    else
                    {
                        Extensions.Remove($"{Part}-{kv.Key}");
                    }
                }
            }

            public void RemoveKeys()
            {
                Extensions.Remove(Part);

                for (int i = 0; i < (PartColors?.Length ?? 0); i++)
                {
                    Extensions.Remove(PartColors[i]);
                }

                foreach (string bundleKey in PartBundles.Values)
                {
                    Extensions.Remove(bundleKey);
                }
            }
        }
    }
}
