using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Weapon : MonoBehaviour
{
    private WeaponItem _weaponItem;
    public WeaponItem weaponItem { get { return _weaponItem; } }
    private new SpriteRenderer renderer;
    // Start is called before the first frame update
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void SwapWeapons(WeaponItem newWeapon)
    {
        _weaponItem = newWeapon;
        renderer.sprite = _weaponItem.art;
    }

    //todo: Handle Attack Animations
}
