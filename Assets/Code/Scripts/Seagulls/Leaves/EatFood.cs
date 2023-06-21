using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.Session;
using ShootingRangeGame.Tags;
using UnityEngine;

namespace ShootingRangeGame.Seagulls.Leaves
{
    public class EatFood : Leaf<SeagullBrain>
    {
        public const float animationLength = 87.0f / 60.0f;

        public static Tag foodTag;
        public float searchRange = 2.0f;

        public bool eaten;
        public float eatTime;

        public override string Name => $"{base.Name} Eat Food";

        public override void Init(BehaviourTree tree)
        {
            base.Init(tree);
            Tag.Cache(ref foodTag, "food");
        }

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

                GameSession.AwardPoint();
                eaten = false;
                return BehaviourTree.Result.Success;
            }

            var seagull = Target.Seagull;
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
            var seagull = Target.Seagull;
            var food = Tree.Blackboard.Get<GameObject>("food");
            if (food) return food;

            var queryList = Physics.OverlapSphere(seagull.transform.position, searchRange);
            foreach (var query in queryList)
            {
                food = query.attachedRigidbody ? query.attachedRigidbody.gameObject : query.gameObject;

                if (!Taggable.HasTag(query.GetComponentInParent<Taggable>(), foodTag)) continue;

                Tree.Blackboard.Set("food", food);
                return food;
            }

            return null;
        }
    }
}