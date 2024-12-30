using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using Object = System.Object;

namespace NoaDebugger
{
    sealed class HierarchyModel : ModelBase
    {
        HashSet<int> _openObjectHashSet = new HashSet<int>();
        HashSet<int> _checkedHashSet = new HashSet<int>();
        int _selectObjectHashCode;
        HierarchyGameObjectEntry _selectObjectEntry;

        List<Scene> _GetScenes()
        {
            List<Scene> allScenes = new List<Scene>();
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                allScenes.Add(SceneManager.GetSceneAt(index));
            }
            return allScenes;
        }

        Scene _GetDontDestroyScene()
        {
            GameObject temp = new GameObject();
            GameObject.DontDestroyOnLoad( temp );
            Scene dontDestroyOnLoad = temp.scene;
            GameObject.DestroyImmediate( temp );
            temp = null;

            return dontDestroyOnLoad;
        }

        List<Transform> _GetTransformsInScene(Scene scene)
        {
            var objects = scene.GetRootGameObjects();
            return objects.Select(go => go.transform).ToList();
        }


        public (HierarchyInformation, HierarchyGameObjectEntry) GetHierarchy()
        {
            HierarchyInformation ret = new HierarchyInformation()
            {
                _objectNum = 0,
                _hierarchySceneList = new List<HierarchyGameObjectEntry>(),
            };
            _selectObjectEntry = null;

            List<Scene> scenes = _GetScenes();
            Scene dontDestroyScene = _GetDontDestroyScene();
            scenes.Add(dontDestroyScene);

            foreach (var scene in scenes)
            {
                List<Transform> transforms = new List<Transform>();
                transforms.AddRange(_GetTransformsInScene(scene));
                List<HierarchyGameObjectEntry> children = _CreateGameObjectEntryList(transforms, ref ret._objectNum);

                int hashCode = scene.GetHashCode();
                bool isOpen = _ContainsOpenObjectHash(scene.GetHashCode());
                bool isSelect = _selectObjectHashCode == hashCode;
                HierarchyGameObjectEntry sceneInformation = new HierarchyGameObjectEntry()
                {
                    _hashCode = scene.GetHashCode(),
                    _name = scene.name,
                    _isActive = scene.isLoaded,
                    _isOpen = isOpen,
                    _isSelected = isSelect,
                    _gameObject = null,
                    _children = children,
                };

                if (isSelect)
                {
                    _selectObjectEntry = sceneInformation;
                }

                ret._hierarchySceneList.Add(sceneInformation);
            }

            _CleaningOpenObjectHash();

            return (ret, _selectObjectEntry);
        }

        List<HierarchyGameObjectEntry> _CreateGameObjectEntryList(List<Transform> transforms, ref int goCount)
        {
            List<HierarchyGameObjectEntry> list = new List<HierarchyGameObjectEntry>();

            foreach (var transform in transforms)
            {
                HierarchyGameObjectEntry goInformation = _CreateGameObjectEntry(transform, ref goCount);

                list.Add(goInformation);
            }
            return list;
        }

        HierarchyGameObjectEntry _CreateGameObjectEntry(Transform transform, ref int goCount)
        {
            int hashCode = transform.gameObject.GetHashCode();
            bool isOpen = _ContainsOpenObjectHash(hashCode);
            bool isSelect = _selectObjectHashCode == hashCode;

            HierarchyGameObjectEntry ret = new HierarchyGameObjectEntry()
            {
                _hashCode = hashCode,
                _name = transform.name,
                _isActive = transform.gameObject.activeInHierarchy,
                _isOpen = isOpen,
                _isSelected = isSelect,
                _gameObject = transform.gameObject,
            };

            if (isSelect)
            {
                _selectObjectEntry = ret;
            }

            ++goCount;

            if (transform.childCount == 0)
            {
                ret._children = null;
                return ret;
            }

            List<HierarchyGameObjectEntry> childList = new List<HierarchyGameObjectEntry>();
            foreach (Transform child in transform)
            {
                HierarchyGameObjectEntry info = _CreateGameObjectEntry(child, ref goCount);
                childList.Add(info);
            }

            ret._children = childList;

            return ret;
        }

        public void AddOpenObjectHash(int hashCode)
        {
            _openObjectHashSet.Add(hashCode);
        }

        public void RemoveOpenObjectHash(int hashCode)
        {
            _openObjectHashSet.Remove(hashCode);
        }

        bool _ContainsOpenObjectHash(int hashCode)
        {
            if (_openObjectHashSet.Contains(hashCode))
            {
                _checkedHashSet.Add(hashCode);
                return true;
            }

            return false;
        }

        void _CleaningOpenObjectHash()
        {
            HashSet<int> removeTargetHashSet = new HashSet<int>();
            foreach (int hashCode in _openObjectHashSet)
            {
                if (!_checkedHashSet.Contains(hashCode))
                {
                    removeTargetHashSet.Add(hashCode);
                }
            }

            foreach(int hashCode in removeTargetHashSet)
            {
                _openObjectHashSet.Remove(hashCode);
            }

            _checkedHashSet.Clear();
        }

        public void SetSelectObjectHashCode(int hashCode)
        {
            _selectObjectHashCode = hashCode;
        }


        public List<GameObjectDetailEntry> GetGameObjectDetailIEntryList(GameObject gameObject)
        {
            List<GameObjectDetailEntry> ret = new List<GameObjectDetailEntry>();

            GameObjectDetailEntry entry = new GameObjectDetailEntry()
            {
                _name = $"{gameObject.name} (GameObject)",
                _isOpen = false,
                _subDetailList = _GetGameObjectDetailList(gameObject)
            };
            ret.Add(entry);

            var components = gameObject.GetComponents<Component>();
            foreach (var component in components)
            {
                GameObjectDetailEntry detailInformation = _GetComponentDetail(component);
                ret.Add(detailInformation);
            }

            return ret;
        }

        List<GameObjectDetailEntry> _GetGameObjectDetailList(GameObject gameObject)
        {
            string[] propertyNames = new []{"IsActive", "Name", "Tag", "Layer"};
            string[] propertyValues = new []{$"{gameObject.activeSelf}", $"{gameObject.name}", $"{gameObject.tag}", $"{gameObject.layer.ToString()}"};

            List<GameObjectDetailEntry> subDetailList = new List<GameObjectDetailEntry>();
            for (int i = 0; i < propertyNames.Length; i++)
            {
                GameObjectDetailEntry subEntry = new GameObjectDetailEntry()
                {
                    _name = propertyNames[i],
                    _value = propertyValues[i],
                    _isOpen = false,
                };
                subDetailList.Add(subEntry);
            }
            return subDetailList;
        }

        GameObjectDetailEntry _GetComponentDetail(Component component)
        {
            GameObjectDetailEntry ret = new GameObjectDetailEntry()
            {
                _name = component.GetType().Name,
                _subDetailList = new List<GameObjectDetailEntry>(),
            };

            Type componentType = component.GetType();
            if (componentType.Namespace != null && componentType.Namespace.Contains("UnityEngine"))
            {
                var properties = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var propertyInfo in properties)
                {
                    if (!HierarchyModelHelper.IsViewPropertyInfo(propertyInfo))
                    {
                        continue;
                    }

                    if (componentType == typeof(Canvas))
                    {
                        if (propertyInfo.Name == "renderingDisplaySize")
                        {
                            continue;
                        }
                    }

                    GameObjectDetailEntry info = _GetComponentProperty(propertyInfo, component);
                    ret._subDetailList.Add(info);
                }
            }

            var fieldInfos = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach( var fieldInfo in fieldInfos )
            {
                if (!HierarchyModelHelper.IsViewFieldInfo(fieldInfo))
                {
                    continue;
                }

                GameObjectDetailEntry info = _GetComponentField(fieldInfo, component);
                ret._subDetailList.Add(info);
            }

            return ret;
        }

        GameObjectDetailEntry _GetComponentProperty(PropertyInfo propertyInfo, Component component)
        {
            GameObjectDetailEntry ret = new GameObjectDetailEntry()
            {
                _name = propertyInfo.Name,
                _value = null,
                _isOpen = false,
                _subDetailList = new List<GameObjectDetailEntry>()
            };

            var type = propertyInfo.PropertyType;
            var depth = 0;

            IParamCreator creator = ParamCreatorFactory.ParamCreatorByType(type);
            ret._value = creator.GetValue(propertyInfo, component);
            if (ret._value == null)
            {
                Object fieldInfoValue = null;
                try
                {
                    fieldInfoValue = propertyInfo.GetValue(component);
                }
                catch
                {

                }

                if (fieldInfoValue == null)
                {
                    return ret;
                }
                ret._subDetailList = creator.CreateSubParameter(fieldInfoValue, component, depth);
            }

            return ret;
        }

        GameObjectDetailEntry _GetComponentField(FieldInfo fieldInfo, Component component)
        {
            GameObjectDetailEntry ret = new GameObjectDetailEntry()
            {
                _name = fieldInfo.Name,
                _value = null,
                _isOpen = false,
                _subDetailList = new List<GameObjectDetailEntry>()
            };

            var type = fieldInfo.FieldType;
            var depth = 0;

            IParamCreator creator = ParamCreatorFactory.ParamCreatorByType(type);
            ret._value = creator.GetValue(fieldInfo, component);
            if (ret._value == null)
            {
                Object fieldInfoValue = fieldInfo.GetValue(component);
                if (fieldInfoValue != null)
                {
                    ret._subDetailList = creator.CreateSubParameter(fieldInfoValue, component, depth);
                }

                if (fieldInfoValue is UnityEngine.Object || (fieldInfoValue == null && type.Namespace != null && type.Namespace.Contains("UnityEngine")))
                {
                    ret._subDetailList.Add(_GetAttachNameFromObject(fieldInfo, fieldInfoValue));
                }
            }

            return ret;
        }

        GameObjectDetailEntry _GetAttachNameFromObject (FieldInfo field, Object obj)
        {
            Type componentType = field.FieldType;
            GameObjectDetailEntry tmp = new GameObjectDetailEntry();
            if (componentType.Namespace != null && componentType.Namespace.Contains("UnityEngine"))
            {
                var vName = "";

                UnityEngine.Object unityObject = (UnityEngine.Object) obj;
                if (unityObject != null)
                {
                    vName = unityObject.name;
                }
                else
                {
                    vName = $"None({componentType})";
                }

                tmp = new GameObjectDetailEntry()
                {
                    _name = "Name",
                    _value = vName,
                    _isOpen = false,
                    _subDetailList = new List<GameObjectDetailEntry>()
                };
            }
            return tmp;
        }

        public sealed class HierarchyInformation
        {
            public List<HierarchyGameObjectEntry> _hierarchySceneList;

            public int _objectNum;
        }

    }
}
