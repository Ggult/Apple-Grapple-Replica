using System.Collections.Generic;
using UnityEngine;

namespace AppleGrapple
{
    public class WeaponPooling : MonoBehaviour
    {
        [SerializeField] private WeaponConfig _weaponConfig;
        [SerializeField] private int _initialPoolSize = 20;
        private static WeaponPooling _instance;
        private readonly Queue<Sword> _pool = new();

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        for (int i = 0; i < _initialPoolSize; i++)
        {
            Create();
        }
    }

        private Sword Create()
    {
        Sword sword = Instantiate(_weaponConfig.weaponPrefab, transform);
        sword.gameObject.SetActive(false);

        _pool.Enqueue(sword);

        return sword;
    }

        public static Sword Get()
    {
        if (_instance._pool.Count == 0)
        {
            _instance.Create();
        }

        Sword sword = _instance._pool.Dequeue();
        sword.gameObject.SetActive(true);

        return sword;
    }
        public static void Return(Sword sword)
    {
        CombatResolver.ClearCooldowns(sword);
        sword.gameObject.SetActive(false);
        sword.transform.SetParent(_instance.transform);

        _instance._pool.Enqueue(sword);
        }
    }
}