﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class EnemySetData
{
    public List<CharcterDBData> _charList;
}


[CreateAssetMenu(fileName = "EnemySetDBData", menuName = "DataBases/Data/EnemySetDBData", order = 0)]
public class EnemySetDBData : AbstractDBData
{
    public EnemySetData _enemySetData=new EnemySetData();

    [SerializeField, NonEditable] List<string> _enemyNameList = new List<string>();

    protected override Dictionary<string, int> InitMember_int()
    {
        return new Dictionary<string, int>();
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        return new Dictionary<string, string>();
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        var result= new Dictionary<string, List<string>>();
        result.Add("enemy", _enemyNameList);
        return result = new Dictionary<string, List<string>>();
    }

    protected override void UpdateMember()
    {
        _enemyNameList = _Data._memberSet_stList["enemy"];
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        var db = SaveDataController.Instance.GetDB_static<CharcterDB>();
        var dbList = db.GetDataList();
        _enemySetData._charList = new List<CharcterDBData>();
        foreach (var skill in _enemyNameList)
        {
            var data = dbList.Where(x => x.name == skill).First();
            _enemySetData._charList.Add(data as CharcterDBData);
        }
        EditorUtility.SetDirty(this);
    }
}
