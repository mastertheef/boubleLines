using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPopup : PopupBase
{
    private ShopController shopController;

    public override void Start()
    {
        base.Start();
        shopController = FindObjectOfType<ShopController>();
    }

    public void Buy(string richId)
    {
        if (shopController != null)
        {
            shopController.BuyConsumable(richId);
        }

        ClosePopup();
    }
}
