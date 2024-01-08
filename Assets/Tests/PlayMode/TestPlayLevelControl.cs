using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayLevelControl
{
    [UnityTest]
    public IEnumerator IsLevelComplete_NoLevelElem_LevelComplete()
    {
        var levelCtrl = new GameObject().AddComponent<LevelController>();

        levelCtrl.IsLevelComplete = false;
        levelCtrl.CheckLevelElementComplete();

        Assert.IsTrue(levelCtrl.IsLevelComplete);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsLevelComplete_ContainPuzzleDone_LevelComplete()
    {
        var levelCtrl = new GameObject().AddComponent<LevelController>();
        var puzzleTrigger = new GameObject().AddComponent<LevelCompleteTrigger>();
        puzzleTrigger.IsPlayerReached = true;

        levelCtrl.IsLevelComplete = false;
        levelCtrl.puzzleCompleteTrigger = puzzleTrigger;
        levelCtrl.CheckLevelElementComplete();

        Assert.IsTrue(levelCtrl.IsLevelComplete);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsLevelComplete_ContainEnemyArea_LevelComplete()
    {
        var levelCtrl = new GameObject().AddComponent<LevelController>();
        var enemyAreaCtrl = new GameObject().AddComponent<EnemyAreaController>();
        enemyAreaCtrl.IsAllSpawnsCleared = true;

        levelCtrl.IsLevelComplete = false;
        levelCtrl.enemyAreaCtrl = enemyAreaCtrl;
        levelCtrl.CheckLevelElementComplete();

        Assert.IsTrue(levelCtrl.IsLevelComplete);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsLevelComplete_PuzzleAndEnemyAreaBothDone_LevelComplete()
    {
        var levelCtrl = new GameObject().AddComponent<LevelController>();
        var puzzleTrigger = new GameObject().AddComponent<LevelCompleteTrigger>();
        puzzleTrigger.IsPlayerReached = true;
        var enemyAreaCtrl = new GameObject().AddComponent<EnemyAreaController>();
        enemyAreaCtrl.IsAllSpawnsCleared = true;

        levelCtrl.IsLevelComplete = false;
        levelCtrl.puzzleCompleteTrigger = puzzleTrigger;
        levelCtrl.enemyAreaCtrl = enemyAreaCtrl;
        levelCtrl.CheckLevelElementComplete();

        Assert.IsTrue(levelCtrl.IsLevelComplete);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsLevelComplete_PuzzleAndEnemyAreaBothNotDone_LevelNotComplete()
    {
        var levelCtrl = new GameObject().AddComponent<LevelController>();
        var puzzleTrigger = new GameObject().AddComponent<LevelCompleteTrigger>();
        puzzleTrigger.IsPlayerReached = false;
        var enemyAreaCtrl = new GameObject().AddComponent<EnemyAreaController>();
        enemyAreaCtrl.IsAllSpawnsCleared = false;

        levelCtrl.IsLevelComplete = false;
        levelCtrl.puzzleCompleteTrigger = puzzleTrigger;
        levelCtrl.enemyAreaCtrl = enemyAreaCtrl;
        levelCtrl.CheckLevelElementComplete();

        Assert.IsFalse(levelCtrl.IsLevelComplete);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsLevelComplete_PuzzleDoneButEnemyAreaNotDone_LevelNotComplete()
    {
        var levelCtrl = new GameObject().AddComponent<LevelController>();
        var puzzleTrigger = new GameObject().AddComponent<LevelCompleteTrigger>();
        puzzleTrigger.IsPlayerReached = true;
        var enemyAreaCtrl = new GameObject().AddComponent<EnemyAreaController>();
        enemyAreaCtrl.IsAllSpawnsCleared = false;

        levelCtrl.IsLevelComplete = false;
        levelCtrl.puzzleCompleteTrigger = puzzleTrigger;
        levelCtrl.enemyAreaCtrl = enemyAreaCtrl;
        levelCtrl.CheckLevelElementComplete();

        Assert.IsFalse(levelCtrl.IsLevelComplete);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsLevelComplete_EnemyAreaDoneButPuzzleNotDone_LevelNotComplete()
    {
        var levelCtrl = new GameObject().AddComponent<LevelController>();
        var puzzleTrigger = new GameObject().AddComponent<LevelCompleteTrigger>();
        puzzleTrigger.IsPlayerReached = false;
        var enemyAreaCtrl = new GameObject().AddComponent<EnemyAreaController>();
        enemyAreaCtrl.IsAllSpawnsCleared = true;

        levelCtrl.IsLevelComplete = false;
        levelCtrl.puzzleCompleteTrigger = puzzleTrigger;
        levelCtrl.enemyAreaCtrl = enemyAreaCtrl;
        levelCtrl.CheckLevelElementComplete();

        Assert.IsFalse(levelCtrl.IsLevelComplete);

        yield return null;
    }

    [TearDown]
    public void AfterEveryTest()
    {
        foreach (var gameObject in GameObject.FindObjectsOfType<GameObject>())
        {
            Object.Destroy(gameObject);
        }
    }
}
