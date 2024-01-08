using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayCombatArea
{
    [UnityTest]
    [TestCase(1, ExpectedResult = null)]
    [TestCase(3, ExpectedResult = null)]
    [TestCase(5, ExpectedResult = null)]
    public IEnumerator IsLockDown_TriggerNotGoesOff_NotLockDown(int spawnAreaAmount)
    {
        GameObject combatArea = _GetCombatArea(spawnAreaAmount);
        EnemyAreaController ememyAreaScript = combatArea.GetComponent<EnemyAreaController>();

        yield return new WaitForSeconds(.1f);

        Assert.IsFalse(ememyAreaScript.IsAllSpawnsCleared);
        Assert.IsFalse(ememyAreaScript.IsLockedDown);
    }

    [UnityTest]
    [TestCase(1, ExpectedResult = null)]
    [TestCase(3, ExpectedResult = null)]
    [TestCase(5, ExpectedResult = null)] 
    public IEnumerator IsLockDown_AnyTriggerGoesOff_LockDown(int spawnAreaAmount)
    {
        GameObject combatArea = _GetCombatArea(spawnAreaAmount);
        EnemyAreaController ememyAreaScript = combatArea.GetComponent<EnemyAreaController>();
        GameObject player = _GetPlayerObject();

        player.transform.position = combatArea.transform.position;

        yield return new WaitForSeconds(.1f);

        Assert.IsFalse(ememyAreaScript.IsAllSpawnsCleared);
        Assert.IsTrue(ememyAreaScript.IsLockedDown);
    }

    [UnityTest]
    public IEnumerator IsEnemyAreaAllClear_EnemyNotAllCleared_NotAllCleared()
    {
        GameObject combatArea = _GetCombatArea(2);
        EnemyAreaController ememyAreaScript = combatArea.GetComponent<EnemyAreaController>();
        GameObject player = _GetPlayerObject();

        player.transform.position = combatArea.transform.position;

        yield return new WaitForSeconds(.1f);

        List<EnemySpawnPoint> enemySps = combatArea.GetComponentsInChildren<EnemySpawnPoint>().ToList();
        enemySps[0].IsEnemyInAreaClear = true;
        enemySps[1].IsEnemyInAreaClear = false;

        Assert.IsFalse(ememyAreaScript.IsAllSpawnsCleared);
        Assert.IsTrue(ememyAreaScript.IsLockedDown);
    }

    [UnityTest]
    public IEnumerator IsEnemyAreaAllClear_EnemyAllCleared_AllCleared()
    {
        GameObject combatArea = _GetCombatArea(2);
        EnemyAreaController ememyAreaScript = combatArea.GetComponent<EnemyAreaController>();
        GameObject player = _GetPlayerObject();

        player.transform.position = combatArea.transform.position;

        yield return new WaitForSeconds(.1f);

        List<EnemySpawnPoint> enemySps = combatArea.GetComponentsInChildren<EnemySpawnPoint>().ToList();
       
        enemySps[0].IsEnemyInAreaClear = true;
        enemySps[1].IsEnemyInAreaClear = true;

        yield return new WaitForSeconds(.1f);

        Assert.IsTrue(ememyAreaScript.IsAllSpawnsCleared);
        Assert.IsFalse(ememyAreaScript.IsLockedDown);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            Object.Destroy(obj);
        }
    }

    private GameObject _GetCombatArea(int spawnAreaAmount)
    {
        GameObject combatArea = new();

        for (int i = 0; i < spawnAreaAmount; i++)
        {
            GameObject spawnArea = new();
            GameObject spawnPoint = new();
            spawnArea.AddComponent<BoxCollider2D>();
            spawnArea.GetComponent<BoxCollider2D>().isTrigger = true;   

            spawnPoint.transform.parent = spawnArea.transform;
            spawnArea.transform.parent = combatArea.transform;

            spawnArea.AddComponent<EnemySpawnPoint>();
        }

        combatArea.AddComponent<EnemyAreaController>();
        return combatArea;
    }
    private GameObject _GetPlayerObject()
    {
        GameObject testObject = new GameObject();

        testObject.AddComponent<BoxCollider2D>();
        testObject.AddComponent<Rigidbody2D>();
        testObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        testObject.layer = 8;

        return testObject;
    }
}
