using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    private const int StageChipSize = 30;

    private int currentChipSize = 30;

    private int currentChipIndex;

    public Transform character;

    public GameObject[] stageChips;

    public int startChipIndex;

    public int preInstantiate;
    public List<GameObject> generatedStageList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        currentChipIndex = startChipIndex - 1;
        UpdateStage(preInstantiate);
    }

    // Update is called once per frame
    void Update()
    {
        //キャラクターのいちから現在のステージチップのインデックスを計算
        int charaPositionIndex = (int) (character.position.z / StageChipSize);
        
        //キャラクターが次のステージチップに入ったらステージの更新処理を行う
        if (charaPositionIndex + preInstantiate > currentChipIndex)
        {
            UpdateStage(charaPositionIndex + preInstantiate);
        }
    }
    
    //指定のIndexまでのステージチップを生成して、管理下に置く
    void UpdateStage(int toChipIndex)
    {
        //引数が現在のチップインデックスより小さい場合はステージチップは作成しない。
        if (toChipIndex <= currentChipIndex)
        {
            return;
        }
        
        //指定のステージチップまで作成
        for (int i = currentChipIndex + 1; i <= toChipIndex ; i++)
        {
            GameObject stageObject = GenerateStage(i);
            
            //生成したステージチップを管理リストに追加
            generatedStageList.Add(stageObject);
        }
        
        //ステージ保持上限内になるまで古いステージを破棄
        while (generatedStageList.Count > preInstantiate + 2)
        {
            DestroyOldestStage();
        }

        currentChipIndex = toChipIndex;
    }

    //指定のインデックス位置にStageオブジェクトをランダムに生成
    GameObject GenerateStage(int chipIndex)
    {
        int nextStageChip = Random.Range(0, stageChips.Length);
        
        GameObject stageObject = (GameObject)Instantiate(
            stageChips[nextStageChip],
            new Vector3(0,0,chipIndex * StageChipSize),
            Quaternion.identity
                );

        return stageObject;
    }

    //1番古いステージを破棄
    void DestroyOldestStage()
    {
        GameObject oldStage = generatedStageList[0];
        generatedStageList.RemoveAt(0);
        Destroy(oldStage);
    }
}
