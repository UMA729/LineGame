using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StrokeController : MonoBehaviour
{
    //通常線関数
    [Header("Line")]
    [SerializeField] Material lineMaterial;
    [SerializeField] Color lineColor;
    [Range(0.1f, 0.5f)]
    [SerializeField] float lineWidth;

    [SerializeField] PhysicsMaterial2D bounceMaterial;
    [SerializeField] LayerMask blockLayer;

    //重量用関数
    [Header("Weight")]
    [SerializeField] GameObject weightPrefab;

    [SerializeField] float weightGrowSpeed = 1f;
    [SerializeField] float weightMaxScale = 3f;

    [SerializeField] float weightMaxMass = 10f;
    [SerializeField] float weightGaugeCost = 10f;

    float weightScale = 1f;
    [SerializeField] string[] WeightTag;


    //共通用関数
    [Header("For General Sharing")]
    [SerializeField] float maxGauge = 100f;
    [SerializeField] float gaugeCostPerUnit = 10f;

    [SerializeField] Image gauge;
    float currentGauge;

    [SerializeField] float lifeTime = 3f;

    //描画タイプ
    public enum LineType
    {
        Normal,
        Weight,
        Spring
    }

    //点オブジェクト時間クラス
    [System.Serializable]
    public class TimedPoint
    {
        public Vector2 position;
        public float time;

        public TimedPoint(Vector2 pos, float t)
        {
            position = pos;
            time = t;
        }
    }

    //線のデータクラス
    [System.Serializable]
    public class LineData
    {

        public GameObject obj;
        public LineRenderer renderer;
        public EdgeCollider2D collider;

        public List<TimedPoint> points = new List<TimedPoint>();

        public float life = 3f;
        public bool released = false;
        public float recoverLockTime = 3f;
        public float releaseTime;
        public int removedCount = 0;
        public int initialPointCount;
        public int startIndex = 0;
        public float totalLength = 0;
        public bool colliderDirty = true;
        public float alpha = 1f;

        // 回復用
        public float recoverStartTime;
        public float recoverAmount;
        public float recoverSpeed;
        public bool recovering = false;

        public float usedGauge;
    }

    [System.Serializable]
    public class WeightData
    {
        public GameObject obj;
        public Rigidbody2D rb;

        public float scale = 1f;

        public bool released;

        public float recoverAmount;

        // この球が使ったゲージ量
        public float usedGauge;

        public string[] tag;
        //縮小フラグ
        public bool isReducing = false;
        public bool isInfrate = true;
    }

    List<LineData> lines = new List<LineData>();
    List<LineData> recoveringLines = new List<LineData>();
    LineData currentLine;

    List<WeightData>weights = new List<WeightData>();
    WeightData currentWeight;

    public bool now_stroke = false;

    public LineType type;
    void Start()
    {
        currentGauge = maxGauge;
        type = LineType.Normal;
        gauge.fillAmount = 1f;
    }

    void Update()
    {
        _updateAllLines();

        _updateweight();

        _updateLineGauge();

        _updateWeightGauge();

        if (!now_stroke)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                type = LineType.Normal;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                type = LineType.Weight;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            now_stroke = true;
            if (type == LineType.Normal)
            {
                _createLine();
            }
            else if (type == LineType.Weight)
            {
                _createweight();
            }

        }
        if (now_stroke)
        {
            if (Input.GetMouseButton(0) && type == LineType.Normal)
            {
                _addPoint();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            now_stroke = false;

            if (currentLine != null　&& type == LineType.Normal)
            {
                currentLine.released = true;
                currentLine.releaseTime = Time.time + 1f; 
                currentLine.initialPointCount = currentLine.points.Count;
                currentLine.colliderDirty = true;
            }

            if (currentWeight != null && type == LineType.Weight)
            {
                currentWeight.released = true;

                Rigidbody2D rb =
                currentWeight.obj.AddComponent<Rigidbody2D>();

                rb.linearVelocity = Vector2.zero;

                rb.mass = currentWeight.scale * 20f;
                rb.gravityScale = 1f;


                currentWeight.rb = rb;
            }

            currentWeight = null;
        }

    }    

    //線オブジェクト作成
    private void _createLine()
    {
        GameObject obj = new GameObject("Line");
        obj.tag = "Line";
        obj.transform.SetParent(transform);

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        EdgeCollider2D col = obj.AddComponent<EdgeCollider2D>();

        lr.material = lineMaterial;
        lr.material.color = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 0;

        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        currentLine = new LineData
        {
            obj = obj,
            renderer = lr,
            collider = col,
        };

        obj.AddComponent<LineCollision>().line_data = currentLine;

        lines.Add(currentLine);
    }

    //重量オブジェクト作成
    void _createweight()
    {
        Vector3 mouse =
            Input.mousePosition;

        Vector3 mousePos = new Vector3(
          Input.mousePosition.x,
          Input.mousePosition.y,
          1f
      );

        Vector3 worldPos =
           Camera.main.ScreenToWorldPoint(mousePos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos, blockLayer);

        if (hit != null)
        {
            return;
        }

        Vector3 pos =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mouse.x,
                    mouse.y,
                    1f
                )
            );


        GameObject obj =
            Instantiate(
                weightPrefab,
                pos,
                Quaternion.identity
            );


        Rigidbody2D rb = null;

        string[] str = WeightTag;

        currentWeight =
            new WeightData()
            {
                obj = obj,
                rb = rb,
                tag = str,
                scale = 1f,
            };

        obj.AddComponent<WeightCollision>().weight_data = currentWeight;

        weights.Add(currentWeight);
    }

    //点追加
    private void _addPoint()
    {
        if (currentLine == null) return;
        if (currentGauge <= 0f) return;

        Vector3 mousePos = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            1f
        );

        Vector3 worldPos =
            Camera.main.ScreenToWorldPoint(mousePos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos, blockLayer);

        if (hit != null)
        {
           
            return;
        }

        if (currentLine.points.Count > 0)
        {
            Vector2 lastPos =
                currentLine.points[currentLine.points.Count - 1].position;

            float dist =
                Vector2.Distance(lastPos, worldPos);


            float cost = dist * gaugeCostPerUnit;


            if (currentGauge < cost)
            {
                now_stroke = false;
                return;
            }

            currentLine.totalLength += dist;

            currentGauge -= cost;
            currentLine.usedGauge += cost;
            currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);
            gauge.fillAmount = currentGauge / maxGauge;

            currentLine.colliderDirty = true;
        }

        currentLine.points.Add(
            new TimedPoint(worldPos, Time.time)
        );
    }
    
    //線処理更新
    private void _updateAllLines()
    {
        float now = Time.time;
        bool isDrawing = Input.GetMouseButton(0);

        for (int l = lines.Count - 1; l >= 0; l--)
        {
            var line = lines[l];


            if (line.life <= 0f)
            {
                float recover = 0f;

                for (int i = 0; i < line.points.Count - 1; i++)
                {
                    recover += Vector2.Distance(
                        line.points[i].position,
                        line.points[i + 1].position
                    ) * gaugeCostPerUnit;
                }


                line.recoverAmount = recover;
                line.recoverStartTime = Time.time + 3f;

                recoveringLines.Add(line);


                Destroy(line.obj);
                lines.RemoveAt(l);
                continue;
            }

            //線削除処理
            if (line.released)
            {

                float elapsed = now - line.releaseTime;
                float t = elapsed / lifeTime;

                int targetRemoveCount =
                    Mathf.FloorToInt(t * line.initialPointCount);

                targetRemoveCount = Mathf.Clamp(targetRemoveCount, 0, line.initialPointCount);

                int removeNow = targetRemoveCount - line.removedCount;
                removeNow = Mathf.Clamp(removeNow, 0, line.points.Count);

 

                if (removeNow > 0)
                {
                    float recovered = 0f;

                    for (int j = 0; j < removeNow; j++)
                    {
                        if (j + 1 < line.points.Count)
                        {
                            Vector2 a = line.points[j].position;
                            Vector2 b = line.points[j + 1].position;
                            recovered += Vector2.Distance(a, b) * gaugeCostPerUnit;
                        }
                    }

                    // 回復禁止時間チェック
                    if (Time.time >= line.recoverLockTime)
                    {
                        currentGauge += recovered; currentGauge =
                            Mathf.Clamp(currentGauge, 0f, maxGauge);

                        gauge.fillAmount = currentGauge / maxGauge;
                    }

                    line.points.RemoveRange(0, removeNow); line.removedCount += removeNow;
                }
            }

            if (line.points.Count < 2 && !isDrawing)
            {
                Destroy(line.obj); lines.RemoveAt(l); continue;
            }

            // 描画更新
            line.renderer.positionCount =
                line.points.Count;


            for (int p = 0; p < line.points.Count; p++)
            {
                line.renderer.SetPosition(
                    p,
                    line.points[p].position);
            }
            // Collider（軽量化済み）
            List<Vector2> pts = new List<Vector2>();

            foreach (var p in line.points)
            {
                pts.Add(p.position);
            }

            line.collider.SetPoints(pts);
        }
    }

    //重量処理更新
    void _updateweight()
    {

        if (!Input.GetMouseButton(0))
            return;

        if (currentWeight == null)
            return;


        if (currentGauge <= 0)
            return;

        // 最大ならゲージを消費しない
        if (currentWeight.scale >= weightMaxScale)
        {
            return;
        }

        float cost =
            weightGaugeCost *
            Time.deltaTime;

        // この球に記録
        currentWeight.usedGauge += cost;

        if (currentGauge < cost)
            return;

        currentGauge -= cost;

        gauge.fillAmount =
            currentGauge / maxGauge;

        // サイズアップ
        currentWeight.scale +=
            weightGrowSpeed *
            Time.deltaTime;


        currentWeight.scale =
            Mathf.Clamp(
                currentWeight.scale,
                1f,
                weightMaxScale
            );


        currentWeight.obj.transform.localScale =
            Vector3.one *
            currentWeight.scale;

        // 質量アップ
        if (currentWeight.rb != null && currentWeight.isInfrate)
        {
            currentWeight.rb.mass =
                currentWeight.scale * (weightGaugeCost/2);
        }
    }

    //線分のゲージ管理
    void _updateLineGauge()
    {
        for (int i = recoveringLines.Count - 1; i >= 0; i--)
        {
            LineData line = recoveringLines[i];

            if (Time.time >= line.recoverStartTime)
            {
                currentGauge += currentLine.recoverSpeed * Time.deltaTime;

                currentGauge = Mathf.Clamp(
                    currentGauge,
                    0,
                    maxGauge
                );

                gauge.fillAmount =
                    currentGauge / maxGauge;


                recoveringLines.RemoveAt(i);
            }
        }
    }

    //重量分のゲージ管理
    void _updateWeightGauge()
    {
        for (int i = weights.Count - 1; i >= 0; i--)
        {
            WeightData weight = weights[i];

            if (!weight.isReducing)
                continue;

            if (weight.released)
            {
                // 徐々に小さくする
                weight.scale -=
                    Time.deltaTime;


                weight.scale =
                    Mathf.Clamp(
                        weight.scale,
                        0,
                        weight.scale
                    );


                weight.obj.transform.localScale =
                    Vector3.one * weight.scale;


                // 質量減少
                weight.rb.mass =
                Mathf.Pow(
                    weight.scale,
                    3
                ) * weightMaxMass;
             

                // 完全消滅
                if (weight.scale <= 0)
                {
                    Destroy(weight.obj);


                    // ゲージ回復
                    currentGauge += weight.usedGauge;

                    currentGauge =
                        Mathf.Clamp(
                            currentGauge,
                            0,
                            maxGauge);

                    gauge.fillAmount =
                        currentGauge / maxGauge;


                    weights.RemoveAt(i);
                }
            }
        }
    }
}
