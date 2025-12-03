using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/PotionHP")]
public class PotionHPSO : ItemBase
{
    public float healRatio = 0.3f;

    public override bool Consumable => true;
}