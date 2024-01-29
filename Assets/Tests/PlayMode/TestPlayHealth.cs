using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayHealth
{
    [UnityTest]
    [TestCase(1, 5, 1, ExpectedResult = null)]
    [TestCase(1, 5, 2, ExpectedResult = null)]
    [TestCase(1, 5, 3, ExpectedResult = null)]
    [TestCase(1, 5, 4, ExpectedResult = null)]
    public IEnumerator HealPlayer_EmptyHealthMoreThanEqualHealAmount_HealthIsHealed(int currentHealth, int maxHealth, int regenAmount)
    {
        GameObject fakePlayer = SetupFakePlayer();

        SetupHealth(currentHealth, maxHealth, regenAmount);

        PlayerHealth.Instance.HealPlayer(regenAmount);

        Assert.AreEqual(currentHealth + regenAmount, PlayerHealth.Instance.CurrentHealth);

        yield return null;
    }

    [UnityTest]
    [TestCase(1, 5, 6, ExpectedResult = null)]
    [TestCase(1, 5, 7, ExpectedResult = null)]
    [TestCase(1, 5, 8, ExpectedResult = null)]
    public IEnumerator HealPlayer_EmptyHealthLessThanHealAmount_HealthIsMaxed(int currentHealth, int maxHealth, int regenAmount)
    {
        GameObject fakePlayer = SetupFakePlayer();

        SetupHealth(currentHealth, maxHealth, regenAmount);

        PlayerHealth.Instance.HealPlayer(regenAmount);

        Assert.AreEqual(maxHealth, PlayerHealth.Instance.CurrentHealth);

        yield return null;
    }

    [UnityTest]
    public IEnumerator HealPlayer_HealthIsFull_HealthIsNotHealed()
    {
        GameObject fakePlayer = SetupFakePlayer();

        SetupHealth(5, 5, 1);

        PlayerHealth.Instance.HealPlayer(1);

        Assert.AreEqual(5, PlayerHealth.Instance.CurrentHealth);

        yield return null;
    }

    [TestCase(5, 5, 1, ExpectedResult = null)]
    [TestCase(5, 5, 2, ExpectedResult = null)]
    [TestCase(5, 5, 3, ExpectedResult = null)]
    [TestCase(5, 5, 4, ExpectedResult = null)]
    public IEnumerator DamagePlayer_HealthIsFull_HealthIsReduced(int currentHealth, int maxHealth, int damage)
    {
        GameObject fakePlayer = SetupFakePlayer();

        SetupHealth(currentHealth, maxHealth, 1);

        PlayerHealth.Instance.DamagePlayer(damage);

        Assert.AreEqual(maxHealth - damage, PlayerHealth.Instance.CurrentHealth);

        yield return null;
    }

    private void SetupHealth(int currentHealth, int maxHealth, int regenAmount)
    {
        PlayerHealth.Instance = new GameObject().AddComponent<PlayerHealth>();
        PlayerHealth.Instance.CurrentHealth = currentHealth;
        PlayerHealth.Instance.MaxHealth = maxHealth;
        PlayerHealth.Instance.regenAmount = regenAmount;
        PlayerHealth.Instance.regenTime = 100f;
        PlayerHealth.Instance.avoidToRegenTime = 100f;
        PlayerHealth.Instance.invincibleTime = 1f;
    }

    private GameObject SetupFakePlayer() 
    {   
        GameObject player = new GameObject();
        player.tag = "PlayerCat";
        return player;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        GameObject.Destroy(PlayerHealth.Instance.gameObject);
        GameObject.Destroy(GameObject.FindGameObjectsWithTag("PlayerCat")[0]);
        yield return null;
    }

}
