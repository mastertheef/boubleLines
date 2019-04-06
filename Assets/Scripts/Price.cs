using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Price : MonoBehaviour
{
    private ShopController shopController;
    [SerializeField] private string productId;

    void Start()
    {
        shopController = FindObjectOfType<ShopController>();
        var text = GetComponent<TMP_Text>();

        if (shopController != null && text != null)
        {
            text.text = shopController.GetPrice(productId);
        }
    }
}
