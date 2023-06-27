using System;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.Tags;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShootingRangeGame.Seagulls.Leaves
{
    public class EatFood : Leaf<BirdBrain>
    {
        public const float animationLength = 87.0f / 60.0f;

        public static Tag foodTag;
        public float searchRange = 2.0f;

        public bool eaten;
        public float eatTime;
        public Action eatCallback;

        public override string Name => $"{base.Name} Eat Food";

        public override void Init(BehaviourTree tree)
        {
            base.Init(tree);
            Tag.Cache(ref foodTag, "food");
        }

        public EatFood SetCallback(Action eatCallback)
        {
            this.eatCallback = eatCallback;
            return this;
        }
        
        public override BehaviourTree.AbandonResponse RespondToAbandonRequest() => eaten ? BehaviourTree.AbandonResponse.CannotAbandon : BehaviourTree.AbandonResponse.WithFailure;
        public override bool OverridePending() => FindFood();
        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            if (eaten)
            {
                if (Time.time - eatTime <= animationLength)
                {
                    Target.Animation = "Eat";
                    return BehaviourTree.Result.Pending;
                }

                eatCallback?.Invoke();
                eaten = false;
                return BehaviourTree.Result.Success;
            }

            var seagull = Target.Bird;
            var food = FindFood();
            if (!food) return BehaviourTree.Result.Failure;

            Target.MoveTowards(food.transform.position, 3.0f);

            if ((food.transform.position - seagull.transform.position).magnitude < 0.5f)
            {
                Object.Destroy(food.gameObject);
                eaten = true;
                eatTime = Time.time;
            }

            return BehaviourTree.Result.Pending;
        }

        private GameObject FindFood()
        {
            var seagull = Target.Bird;
            var food = Tree.Blackboard.Get<GameObject>("food");
            if (food) return food;

            var queryList = Physics.OverlapSphere(seagull.transform.position, searchRange);
            foreach (var query in queryList)
            {
                food = query.attachedRigidbody ? query.attachedRigidbody.gameObject : query.gameObject;
                if (food.transform.position.y < CheckForWater.WaterLevel) continue;
                
                if (!Taggable.HasTag(query.GetComponentInParent<Taggable>(), foodTag)) continue;

                Tree.Blackboard.Set("food", food);
                return food;
            }

            return null;
        }
    }
}