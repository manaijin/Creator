using Creator;
using DG.Tweening;
using Framework;
using Framework.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Framework.InputType;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using System;
using Debug = Framework.Util.Debug;
using Unity.Burst;

[BurstCompile]
public struct CalcPointJob : IJobParallelFor
{
    public NativeArray<float3> pos;
    public NativeArray<float> times;
    public NativeArray<float3> curve_points;
    public float deltaTime;
    public float max_deltaTime;
    public void Execute(int index)
    {
        int point_leng = 11;
        int start_index = index * pos.Length;

        // 获取节点，计算位置
        NativeArray<float3> curve_point = new NativeArray<float3>(point_leng, Allocator.Temp);
        for (int i = 0; i < point_leng; i++)
        {
            //try
            //{
            //    curve_point[i] = curve_points[start_index + i];
            //}
            //finally { }
        }
        //pos[index] = MathfUtil.BezierCurve(curve_point, times[index]);
        pos[index] = pos[index] + new float3(0, deltaTime, 0);
        curve_point.Dispose();

        times[index] += deltaTime;
        if (times[index] >= max_deltaTime)
        {
            times[index] = 0;
        }
    }
}

public class Test : MonoBehaviour
{
    private string key = "UI_GameStart";
    private string key2 = "cube1";
    public Mesh mesh;
    public Material mat;
    public Camera cam;
    public GameObject prefab;
    List<Vector3> poslist;


    // Start is called before the first frame update
    public void Start()
    {
        TestJobSysteInit();
    }

    private Transform[] instance_list;
    private float[] times;
    private float3[,] curve_points;
    void TestJobSysteInit()
    {
        if (prefab == null) return;
        instance_list = new Transform[10];
        times = new float[10];
        curve_points = new float3[10, 11];
        for (int i = 0; i < instance_list.Length; i++)
        {
            var go = GameObject.Instantiate(prefab);
            float x = UnityEngine.Random.Range(-10f, 10f);
            float y = UnityEngine.Random.Range(-10f, 10f);
            float z = UnityEngine.Random.Range(-10f, 10f);
            go.transform.position = new Vector3(x, y, z);
            instance_list[i] = go.transform;
            GeneratePoints(i);
        }
    }

    void GeneratePoints(int i)
    {
        curve_points[i, 0] = instance_list[i].position;
        for (int j = 1; j < curve_points.GetLength(1); j++)
        {
            float offset_x = UnityEngine.Random.Range(-10f, 10f);
            float offset_y = UnityEngine.Random.Range(-10f, 10f);
            float offset_z = UnityEngine.Random.Range(-10f, 10f);
            curve_points[i, j] = curve_points[i, 0] + new float3(offset_x, offset_y, offset_z);
        }
    }

    void TestJobSystemUpdate()
    {
        if (prefab == null) return;
        // 重新生成节点
        for(int i = 0; i < this.times.Length; i++)
        {
            if(this.times[i] == 0)
            {
                GeneratePoints(i);
            }
        }

        // 数值传递
        var point_length = this.curve_points.GetLength(1);
        NativeArray<float3> pos = new NativeArray<float3>(instance_list.Length, Allocator.TempJob);
        NativeArray<float> times = new NativeArray<float>(instance_list.Length, Allocator.TempJob);
        NativeArray<float3> curve_points = new NativeArray<float3>(instance_list.Length * point_length, Allocator.TempJob);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = instance_list[i].transform.position;
            times[i] = this.times[i];
            for (int j = 0; j < point_length; j++)
            {
                curve_points[i * point_length + j] = this.curve_points[i, j];
            }
        }

        // Job处理
        var job = new CalcPointJob()
        {
            pos = pos,
            times = times,
            curve_points = curve_points,
            deltaTime = Time.deltaTime,
            max_deltaTime = 10,          
        };
        var handler = job.Schedule(instance_list.Length, 5);
        handler.Complete();

        // 传出Job
        for (int i = 0; i < pos.Length; i++)
        {
            instance_list[i].position = pos[i];
            this.times[i] = times[i];
            for (int j = 0; j < point_length; j++)
            {
                this.curve_points[i, j] = curve_points[i * point_length + j];
            }
        }

        // 变量销毁
        pos.Dispose();
        times.Dispose();
        curve_points.Dispose();
    }


    void TestEcs()
    {
        //var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var archetype = manager.CreateArchetype(
        //    typeof(Translation),
        //    typeof(RenderMesh),
        //    typeof(LocalToWorld),
        //    typeof(RenderBounds),
        //    typeof(MoveData)
        //);
        //NativeArray<Entity> array = new NativeArray<Entity>(10, Allocator.Temp);
        //manager.CreateEntity(archetype, array);
        //for (int i = 0; i < array.Length; i++)
        //{
        //    var entity = array[i];
        //    float x = UnityEngine.Random.Range(-10f, 10f);
        //    float y = UnityEngine.Random.Range(-10f, 10f);
        //    float z = UnityEngine.Random.Range(-10f, 10f);
        //    manager.SetComponentData(entity, new Translation { Value = new float3(x, y, z) });
        //    manager.SetSharedComponentData(entity, new RenderMesh { mesh = mesh, material = mat });
        //    manager.AddBuffer<PointBuffer>(entity);
        //}
        //array.Dispose();
    }

    private void TestDOBezier()
    {
        DOBezierCurve(transform, new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(5, 10, 0),
            new Vector3(15, -10, 0),
            new Vector3(20, 0, 0),
        }, 5);

        //Tween();
    }

    private void DOBezierCurve(Transform trans, Vector3[] points, float duration)
    {
        ulong n = (ulong)(points.Length - 1);
        float t = 0;
        poslist = new List<Vector3>();
        var handler = DOTween.To(() => t, dt =>
        {
            t = dt;
            trans.position = MathfUtil.BezierCurve(points, t);
            poslist.Add(trans.position);
        }, 1, duration);
        handler.SetEase(Ease.Linear);
    }

    private void Tween()
    {
        DOTween.To(() => transform.position, x =>
        {
            transform.position = x;
            UnityEngine.Debug.LogError("Tween");
        }, new Vector3(1, 2, 3), 10);
    }

    private IEnumerator OriginalApi()
    {
        var h1 = Addressables.LoadResourceLocationsAsync(new List<object> { key });
        var h2 = Addressables.LoadResourceLocationsAsync(new List<object> { key2 });
        yield return new WaitUntil(() => { return h1.IsDone && h2.IsDone; });

        var r1 = Addressables.LoadAssetAsync<UnityEngine.Object>(key);
        var r2 = Addressables.LoadAssetAsync<UnityEngine.Object>(key2);
        yield return new WaitUntil(() => { return r1.IsDone && r2.IsDone; });

        var ins1 = Addressables.InstantiateAsync(key);
        var ins2 = Addressables.InstantiateAsync(key2);
        yield return new WaitUntil(() => { return ins1.IsDone && ins2.IsDone; });

        Addressables.Release(r1);
        Addressables.Release(r2);
    }

    private void Update()
    {
        TestJobSystemUpdate();
    }

    private void testInput()
    {
        var combination = new InputCombination<MouseButton>(MouseButton.leftButton, KeyState.wasPressedThisFrame);
        InputMgr.Instance.RegistMouseCallBack(combination, () => { print("单击"); });

        var combination2 = new InputCombination<MouseButton>(MouseButton.leftButton, KeyState.wasDoublePressedThisFrame);
        InputMgr.Instance.RegistMouseCallBack(combination2, () => { print("双击"); });
    }

    private void OnGUI()
    {
        if (mat == null) return;
        if (poslist == null || poslist.Count == 0) return;
        GL.PushMatrix();
        mat.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        for (int i = 0; i < poslist.Count - 1; ++i)
        {
            GL.Vertex(poslist[i]);
            GL.Vertex(poslist[i + 1]);
        }
        GL.End();
        GL.PopMatrix();
    }
}