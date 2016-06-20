using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEditor.Experimental;
using UnityEditor.Experimental.Graph;
using Object = UnityEngine.Object;

namespace UnityEditor.Experimental.VFX
{
    internal class VFXUISpawnerNode : VFXEdNode, VFXModelHolder
    {
        public VFXSpawnerNodeModel Model { get { return m_Model; } }
        public VFXElementModel GetAbstractModel() { return Model; }
        private VFXSpawnerNodeModel m_Model;

        public VFXUIPropertySlotField[] Fields { get { return m_Fields; } }
        protected VFXUIPropertySlotField[] m_Fields;

        internal VFXUISpawnerNode(VFXSpawnerNodeModel model, VFXEdDataSource dataSource) 
            : base (model.UIPosition, dataSource)
        {
            m_Model = model;
            scale = new Vector2(VFXEditorMetrics.NodeDefaultWidth, 100);

            m_Inputs.Add(new VFXEdFlowAnchor(0, typeof(float), VFXContextDesc.Type.kTypeNone, m_DataSource, Direction.Input));
            m_Inputs.Add(new VFXEdFlowAnchor(1, typeof(float), VFXContextDesc.Type.kTypeNone, m_DataSource, Direction.Input));

            m_Outputs.Add(new VFXEdFlowAnchor(2, typeof(float), VFXContextDesc.Type.kTypeNone, m_DataSource, Direction.Output));

            AddChild(inputs[0]);
            AddChild(inputs[1]);
            AddChild(outputs[0]);

            ZSort();
            Layout();
        }

        protected override MiniMenu.MenuSet GetNodeMenu(Vector2 mousePosition)
        {
            MiniMenu.MenuSet menu = new MiniMenu.MenuSet();
            menu.AddItem("Not Implemented", new MiniMenu.HeaderItem("Check Back Later!"));
            return menu;
        }

        public override void OnAddNodeBlock(VFXEdNodeBlock nodeblock, int index)
        {
            throw new NotImplementedException();
        }

        public override bool AcceptNodeBlock(VFXEdNodeBlockDraggable block)
        {
            return Model.CanAddChild(block.GetAbstractModel());
        }

        public override void UpdateModel(UpdateType t)
        {
            Model.UpdatePosition(translation);
        }

        public virtual float GetHeight()
        {
            float height = VFXEditorMetrics.NodeBlockHeaderHeight;
            foreach (var field in m_Fields)
            {
                height += field.scale.y + VFXEditorMetrics.NodeBlockParameterSpacingHeight;
            }
            height += VFXEditorMetrics.NodeBlockFooterHeight;
            return height;
        }

        public override void Render(Rect parentRect, Canvas2D canvas)
        {
            Rect r = m_ClientArea;

            GUI.Box(r, "", VFXEditor.styles.Node);
            GUI.Label(new Rect(0, r.y, r.width, 24), "Spawner", VFXEditor.styles.NodeTitle);

            base.Render(parentRect, canvas);
        }
    }

    internal class VFXUISpawnerBlock : VFXEdNodeBlockDraggable
    {
        public VFXSpawnerBlockModel Model { get { return m_Model; } }
        public override VFXElementModel GetAbstractModel() { return Model; }
        private VFXSpawnerBlockModel m_Model;

        internal VFXUISpawnerBlock(VFXSpawnerBlockModel model, VFXEdDataSource dataSource)
            : base(dataSource)
        {
            m_Model = model;
            collapsed = Model.UICollapsed;

            int nbSlots = Model.GetNbInputSlots();
            m_Fields = new VFXUIPropertySlotField[nbSlots];
            for (int i = 0; i < nbSlots; ++i)
            {
                m_Fields[i] = new VFXUIPropertySlotField(dataSource, Model.GetInputSlot(i));
                AddChild(m_Fields[i]);
            }

            var header = new VFXEdNodeBlockHeaderSimple(VFXSpawnerBlockModel.TypeToName(model.SpawnerType), null, model.GetNbInputSlots() > 0);
            AddChild(header);
        }

        public override void UpdateModel(UpdateType t)
        {
            Model.UpdateCollapsed(collapsed);
        }

        // TODO Not sure this is needed anymore, remove that ?
        public override VFXPropertySlot GetSlot(string name) { return null; }
        public override void SetSlotValue(string name, VFXValue value) {}
    }
}
