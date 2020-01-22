using System.Collections.Generic;
using DPong.Core.UI.Holder;
using UnityEngine;

namespace DPong.Core.UI {
  public class UISystem {
    private readonly Transform _root;
    private readonly Dictionary<UILayer, Transform> _layers = new Dictionary<UILayer, Transform>();

    private readonly UISystemResources _resources;

    public UISystem(Canvas canvas) {
      _root = canvas.transform;
      _resources = Resources.Load<UISystemResources>("UISystemResources");

      foreach (var layer in new[] {UILayer.Background, UILayer.Panels, UILayer.Windows, UILayer.Foreground})
        CreateLayer(layer);
    }

    private void CreateLayer(UILayer layer) {
      var go = new GameObject(layer.ToString(), typeof(RectTransform));

      var rt = go.GetComponent<RectTransform>();
      rt.SetParent(_root);

      rt.anchorMin = Vector2.zero;
      rt.anchorMax = Vector2.one;
      rt.offsetMin = Vector2.zero;
      rt.offsetMax = Vector2.zero;

      _layers.Add(layer, rt);
    }

    public THolder InstantiateHolder<THolder>(THolder holder, UILayer layer, bool visible) where THolder : UIHolder {
      var holderInstance = Object.Instantiate(holder, _layers[layer]);
      holderInstance.InternalInit(visible);

      return holderInstance;
    }

    public T InstantiateInLayer<T>(T prefab, UILayer layer) where T : MonoBehaviour {
      return Object.Instantiate(prefab, _layers[layer]);
    }

    public TWrapper InstantiateHolder<THolder, TWrapper>(THolder holder, TWrapper wrapper, UILayer layer, bool visible)
      where THolder : UIHolder where TWrapper : UIHolderWrapper {
      var holderInstance = Object.Instantiate(holder, _layers[layer]);
      holderInstance.InternalInit(visible);

      var contentInstance = Object.Instantiate(wrapper, holderInstance.ContentRoot);
      contentInstance.InternalInit(holderInstance);

      return contentInstance;
    }

    public TWrapper InstantiateWindow<TWrapper>(WindowType type, TWrapper wrapper, bool visible)
      where TWrapper : UIHolderWrapper {
      return InstantiateHolder(_resources.Windows[type], wrapper, UILayer.Windows, visible);
    }

    public TWrapper InstantiatePanel<TWrapper>(PanelType type, TWrapper wrapper, bool visible)
      where TWrapper : UIHolderWrapper {
      return InstantiateHolder(_resources.Panels[type], wrapper, UILayer.Panels, visible);
    }
  }
}