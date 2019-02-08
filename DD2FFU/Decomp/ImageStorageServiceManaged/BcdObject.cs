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

        public string Name { get; set; }

        public Guid Id { get; set; }

         public uint Type { get; private set; }

        public List<BcdElement> Elements { get; } = new List<BcdElement>();

        public void ReadFromRegistry(OfflineRegistryHandle objectKey)
        {
            var offlineRegistryHandle1 = (OfflineRegistryHandle) null;
            var offlineRegistryHandle2 = (OfflineRegistryHandle) null;
            try
            {
                offlineRegistryHandle1 = objectKey.OpenSubKey("Description");
                Type = (uint) offlineRegistryHandle1.GetValue("Type", 0);
            }
            catch (ImageStorageException ex)
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
                foreach (var subKeyName in offlineRegistryHandle2.GetSubKeyNames())
                {
                    var elementKey = (OfflineRegistryHandle) null;
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
            catch (ImageStorageException ex)
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
            foreach (var element1 in Elements)
                if (element1.DataType == element.DataType)
                    throw new ImageStorageException(string.Format(
                        "{0}: A bcd element with the given datatype already exists.",
                        MethodBase.GetCurrentMethod().Name));
            Elements.Add(element);
        }

        public Guid GetDefaultObjectId()
        {
            var empty = Guid.Empty;
            foreach (var element in Elements)
                if (element.DataType.Equals(BcdElementDataTypes.DefaultObject))
                {
                    var bcdElementObject = element as BcdElementObject;
                    if (bcdElementObject != null)
                        return bcdElementObject.ElementObject;
                }

            return empty;
        }

        
        public void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "BCD Object: {{{0}}}", (object) Id);
            if (BcdObjects.BootObjectList.ContainsKey(Id))
                logger.LogInfo(str + "Friendly Name: {0}", (object) BcdObjects.BootObjectList[Id].Name);
            logger.LogInfo("");
            foreach (var element in Elements)
            {
                element.LogInfo(logger, checked(indentLevel + 2));
                logger.LogInfo("");
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
                return Name;
            return "Unnamed BcdObject";
        }
    }
}