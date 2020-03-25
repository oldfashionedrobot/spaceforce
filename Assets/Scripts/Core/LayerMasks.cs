using UnityEngine;

namespace SpaceForce.Core {

  [CreateAssetMenu(fileName = "LayerMasks", menuName = "SpaceForce/LayerMasks", order = 0)]
  public class LayerMasks : ScriptableObject {
    public LayerMask groundLayerMask;
    public LayerMask shootLayerMask;
  }
}