/*
	Auto Generated By SampleECS,Don't Modify It Manually!
*/
namespace SampleECS
{
	public partial class ECS_Entity
	{
		const int INVAILD_IDX = -1;
		/* Component Code : TestComp */
		public TestComp testcomp{ get { return context.pool_TestComp.use_coms[poolIndecies[ECS_Component_Type.TestComp]-1].user_struct; } set {context.pool_TestComp.use_coms[poolIndecies[ECS_Component_Type.TestComp]-1].user_struct = value; }}
		public bool has_TestComp { get { return poolIndecies[ECS_Component_Type.TestComp]-1 != INVAILD_IDX; } }
		public void Add_TestComp(TestComp com) { if(poolIndecies[ECS_Component_Type.TestComp]-1 == INVAILD_IDX) {poolIndecies[ECS_Component_Type.TestComp] = context.pool_TestComp.NewComponent(com) +1; context.OnEntityChange(this); } }
		public void Replace_TestComp(TestComp com)
		{
			if (dirtyMarkPtr < 0) { context.excute_ptr++; context.excuteEntities[context.excute_ptr] = idx; }
			testcomp = com; dirtyMarkPtr++; dirtyMark[dirtyMarkPtr] = ECS_Component_Type.TestComp;
		}

	}
}
