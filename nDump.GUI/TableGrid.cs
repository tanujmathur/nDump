﻿using System.Collections.Generic;
using System.Windows.Forms;

namespace nDump.GUI
{
    public partial class TableGrid : UserControl
    {
        private const string HasIdentity = "HasIdentity";
        private const string IgnoredColumns = "Ignored Columns";
        private const string TableName = "TableName";
        private const string SelectField = "Select";
        private const string DeleteOnly = "DeleteOnly";

        public TableGrid()
        {
            InitializeComponent();
            AddColumnsToDataGrid(selectDataGridView);
            selectDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public List<SqlTableSelect> SelectList
        {
            get { return (List<SqlTableSelect>) selectDataGridView.DataSource; }
            set { selectDataGridView.DataSource = value; }
        }

        private void AddColumnsToDataGrid(DataGridView dataGridView)
        {
            var tableName = new DataGridViewTextBoxColumn();
            var select = new DataGridViewTextBoxColumn();
            var hasIdentity = new DataGridViewCheckBoxColumn();
            var deleteOnly = new DataGridViewCheckBoxColumn();
            var ignoredColumns = new DataGridViewTextBoxColumn();
            tableName.DataPropertyName = TableName;
            tableName.HeaderText = "Table Name";
            tableName.Name = TableName;
            select.DataPropertyName = SelectField;
            select.HeaderText = "Filtering Select";
            select.Name = SelectField;
            
            hasIdentity.DataPropertyName = HasIdentity;
            hasIdentity.HeaderText = "Has Identity";
            hasIdentity.Name = HasIdentity;
            hasIdentity.Resizable = DataGridViewTriState.True;
            hasIdentity.SortMode = DataGridViewColumnSortMode.Automatic;
            deleteOnly.DataPropertyName = DeleteOnly;
            deleteOnly.HeaderText = "Delete Only";
            deleteOnly.Name = DeleteOnly;
            deleteOnly.Resizable = DataGridViewTriState.True;
            deleteOnly.SortMode = DataGridViewColumnSortMode.Automatic;
            ignoredColumns.HeaderText = IgnoredColumns;
            ignoredColumns.DataPropertyName = "CommaSeparatedExcludedColumns";
            ignoredColumns.Name = "IgnoredColumns";
            ignoredColumns.ReadOnly = true;
            dataGridView.Columns.AddRange(new DataGridViewColumn[]
                                              {
                                                  tableName,
                                                  select,
                                                  hasIdentity,
                                                  deleteOnly,
                                                  ignoredColumns
                                              });
        }

        private void MoveUpButton_Click(object sender, System.EventArgs e)
        {
            if (selectDataGridView.CurrentRow == null) return;
            var selectedItem = selectDataGridView.CurrentRow.Index;
            SwapItems(selectedItem, selectedItem-1);
        }

        private void SwapItems(int sourceIndex, int targetIndex)
        {
            if (sourceIndex<0|| targetIndex<0) return;
            if (sourceIndex >= SelectList.Count || targetIndex >= SelectList.Count) return;
            var rowAbove = SelectList[targetIndex];
            var rowToMove = SelectList[sourceIndex];
            SelectList[sourceIndex] = rowAbove;
            SelectList[targetIndex] = rowToMove;
            selectDataGridView.Refresh();
            selectDataGridView.CurrentCell = selectDataGridView.Rows[targetIndex].Cells[1];
            selectDataGridView.Rows[targetIndex].Selected = true;
        }

        private void MoveDownButton_Click(object sender, System.EventArgs e)
        {
            if (selectDataGridView.CurrentRow == null) return;
            var selectedItem = selectDataGridView.CurrentRow.Index;
            SwapItems(selectedItem, selectedItem+1);
            
        }

        private void RemoveButton_Click(object sender, System.EventArgs e)
        {
            if (selectDataGridView.CurrentRow == null) return;
            SelectList.RemoveAt(selectDataGridView.CurrentRow.Index);
            selectDataGridView.Refresh();
        }
    }
}