using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/PotionMP")]
public class PotionMPSO : ItemBase
{
    public float healRatio = 0.3f;

    public override bool Consumable => true;
}