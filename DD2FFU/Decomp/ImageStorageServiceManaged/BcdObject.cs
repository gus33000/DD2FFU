// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdObject
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdObject
    {
        public BcdObject(string objectName)
        {
            Name = objectName;
            try
            {
                Id = new Guid(objectName);
            }
            catch (Exception ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Object '{1}' isn't a valid ID.", MethodBase.GetCurrentMethod().Name,
                        objectName), ex);
            }
        }


        public BcdObject(Guid objectId, uint dataType)
        {
            Id = objectId;
            Name = string.Format("{{{0}}}", objectId);
            Type = dataType;
        }

        public string Name
        {
            get; set;
        }

        public Guid Id
        {
            get; set;
        }

        public uint Type
        {
            get; private set;
        }

        public List<BcdElement> Elements { get; } = [];

        public void ReadFromRegistry(OfflineRegistryHandle objectKey)
        {
            OfflineRegistryHandle offlineRegistryHandle1 = null;
            OfflineRegistryHandle offlineRegistryHandle2 = null;
            try
            {
                offlineRegistryHandle1 = objectKey.OpenSubKey("Description");
                Type = (uint)offlineRegistryHandle1.GetValue("Type", 0);
            }
            catch (ImageStorageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: There was a problem accessing the Description key for object {1}",
                        MethodBase.GetCurrentMethod().Name, Name), ex);
            }
            finally
            {
                offlineRegistryHandle1?.Close();
            }

            try
            {
                offlineRegistryHandle2 = objectKey.OpenSubKey("Elements");
                foreach (string subKeyName in offlineRegistryHandle2.GetSubKeyNames())
                {
                    OfflineRegistryHandle elementKey = null;
                    try
                    {
                        elementKey = offlineRegistryHandle2.OpenSubKey(subKeyName);
                        Elements.Add(BcdElement.CreateElement(elementKey));
                    }
                    catch (Exception ex)
                    {
                        throw new ImageStorageException(
                            string.Format("{0}: There was a problem accessing element {1} for object {{{2}}}",
                                MethodBase.GetCurrentMethod().Name, subKeyName, Name), ex);
                    }
                    finally
                    {
                        elementKey?.Close();
                    }
                }
            }
            catch (ImageStorageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: There was a problem accessing the Elements key for object {1}",
                        MethodBase.GetCurrentMethod().Name, Name), ex);
            }
            finally
            {
                offlineRegistryHandle2.Close();
            }
        }

        public void AddElement(BcdElement element)
        {
            foreach (BcdElement element1 in Elements)
            {
                if (element1.DataType == element.DataType)
                {
                    throw new ImageStorageException(string.Format(
                                        "{0}: A bcd element with the given datatype already exists.",
                                        MethodBase.GetCurrentMethod().Name));
                }
            }

            Elements.Add(element);
        }

        public Guid GetDefaultObjectId()
        {
            Guid empty = Guid.Empty;
            foreach (BcdElement element in Elements)
            {
                if (element.DataType.Equals(BcdElementDataTypes.DefaultObject))
                {
                    if (element is BcdElementObject bcdElementObject)
                    {
                        return bcdElementObject.ElementObject;
                    }
                }
            }

            return empty;
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "BCD Object: {{{0}}}", Id);
            if (BcdObjects.BootObjectList.ContainsKey(Id))
            {
                logger.LogInfo(str + "Friendly Name: {0}", BcdObjects.BootObjectList[Id].Name);
            }

            logger.LogInfo("");
            foreach (BcdElement element in Elements)
            {
                element.LogInfo(logger, checked(indentLevel + 2));
                logger.LogInfo("");
            }
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Name) ? Name : "Unnamed BcdObject";
        }
    }
}