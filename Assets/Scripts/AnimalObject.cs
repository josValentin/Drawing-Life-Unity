using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimalObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private float speed = 10f;
    private Transform waipointsRoot;
    private int previousIndex, targetIndex;

    Vector2 lastPos;

    Vector2 limitLeft;
    Vector2 limitRight;

    Vector2 randPos;

    bool waitTime = false;

    Animator anim;
    float traveled = 0;
    private Dictionary<int, float> indexToScale = new Dictionary<int, float>()
    {
        {0, 0.5f },
        {1, 0.7f },
        {2, 1.0f },
    };

    private void Start()
    {
        waipointsRoot = GameObject.Find("Waypoints").transform;
        transform.position = waipointsRoot.GetChild(0).position;
        transform.localScale = Vector2.one * 0.5f;

        previousIndex = 0;
        targetIndex = 1;

        limitLeft = GameObject.Find("Limits/Left").transform.position;
        limitRight = GameObject.Find("Limits/Right").transform.position;

        randPos = new Vector2(Random.Range(limitLeft.x, limitRight.x), Random.Range(limitLeft.y, limitRight.y));

        anim = GetComponentInChildren<Animator>();
    }

    public void SetSkin(Texture2D tex)
    {
        //spr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        var mat = spr.material;
        mat.SetTexture("_FaceTex", tex);
    }

    private void Update()
    {
        if (targetIndex < waipointsRoot.childCount)
        {
            Vector2 startPos = waipointsRoot.GetChild(previousIndex).position;
            Vector2 targetPos = waipointsRoot.GetChild(targetIndex).position;

            traveled += speed * Time.deltaTime;
            float tValue = traveled / Vector3.Distance(startPos, targetPos);
            tValue = Mathf.Clamp01(tValue);

            transform.position = Vector2.Lerp(startPos, targetPos, tValue);

            //bool isMovingDown = previousIndex < targetIndex;
            transform.SetLocalScale(Mathf.Lerp(indexToScale[previousIndex], indexToScale[targetIndex], tValue));

            if (tValue >= 1f)
            {
                previousIndex = targetIndex;
                targetIndex++;
                tValue = 0f;
                traveled = 0;
            }
        }
        else
        {
            if (!waitTime)
            {
                transform.position = Vector2.MoveTowards(transform.position, randPos, Time.deltaTime * speed);

                if (Vector2.Distance(transform.position, randPos) < 0.05f)
                {
                    randPos = new Vector2(Random.Range(limitLeft.x, limitRight.x), Random.Range(limitLeft.y, limitRight.y));

                    waitTime = true;
                    this.ActionAfterTime(Random.Range(2f, 5f), () =>
                    {
                        waitTime = false;
                    });
                }
            }
        }

        Vector2 movement = ((Vector2)transform.position - lastPos).normalized;

        if (movement.x > 0)
        {
            spr.flipX = true;
        }
        else if (movement.x < 0)
        {
            spr.flipX = false;
        }

        anim.SetBool("isWalking", movement.magnitude > 0);

        lastPos = transform.position;
    }
}
