﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSage.Data.Apt.Characters;
using OpenSage.Gui.Apt.ActionScript.Library;

namespace OpenSage.Gui.Apt.ActionScript
{
    public class ObjectContext
    {
        /// <summary>
        /// The item that this context is connected to
        /// </summary>
        public IDisplayItem Item { get; private set; }

        /// <summary>
        /// Contains functions and member variables
        /// </summary>
        public Dictionary<string, Value> Variables { get; set; }

        public List<Value> Constants { get; set; }

        /// <summary>
        /// this ActionScript object is not bound bound to an item, e.g. for global object
        /// </summary>
        public ObjectContext()
        {
            //Actionscript variables are not case sensitive!
            Variables = new Dictionary<string, Value>(StringComparer.OrdinalIgnoreCase);
            Constants = new List<Value>();
        }

        /// <summary>
        /// this ActionScript object is bound to an item
        /// </summary>
        /// <param name="item"></param>
        /// the item that this context is bound to
        public ObjectContext(IDisplayItem item)
        {
            Variables = Variables = new Dictionary<string, Value>(StringComparer.OrdinalIgnoreCase);
            Constants = new List<Value>();
            Item = item;

            //initialize item dependent properties
            InitializeProperties();
        }

        /// <summary>
        /// return a variable, when not present return Undefined
        /// </summary>
        /// <param name="name">variable name</param>
        /// <returns></returns>
        public virtual Value GetMember(string name)
        {
            Value result;

            if (!Variables.TryGetValue(name, out result))
            {
                //Check if this is a property
                if(Property.StringMapping.ContainsKey(name))
                {
                    return GetProperty(Property.StringMapping[name]);
                }

                Debug.WriteLine("[WARN] Undefined variable: " + name);
                result = Value.Undefined();
            }

            return result;
        }

        /// <summary>
        /// Check wether or not a string is a builtin flash function
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns></returns>
        public virtual bool IsBuiltInFunction(string name)
        {
            return Builtin.IsBuiltInFunction(name);
        }

        /// <summary>
        /// Execute a builtin function maybe move builtin functions elsewhere
        /// </summary>
        /// <param name="name"><function name/param>
        public virtual void CallBuiltInFunction(string name, Value[] args)
        {
            Builtin.CallBuiltInFunction(name, this, args);
        }

        private void InitializeProperties()
        {
            //TODO: avoid new fancy switch
            switch (Item.Character)
            {
                case Text t:
                    Variables["textColor"] = Value.FromString(t.Color.ToHex());
                    break;
            }
        }

        /// <summary>
        /// used by text
        /// </summary>
        /// <param name="value">value name</param>
        /// <returns></returns>
        public Value ResolveValue(string value,ObjectContext ctx)
        {
            var path = value.Split('.');
            var obj = this;
            var member = path.First();

            if(path.Length>1)
            {
                if (Builtin.IsBuiltInVariable(path.First()))
                {
                    obj = Builtin.GetBuiltInVariable(path.First(),ctx).ToObject();
                    member = path[1];
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return obj.GetMember(member);
        }

        public ObjectContext GetParent()
        {
            ObjectContext result = null;

            if(Item.Parent!=null)
            {
                result = Item.Parent.ScriptObject;
            }

            return result;
        }

        public Value GetProperty(Property.Type property)
        {
            Value result = null;

            switch (property)
            {
                case Property.Type.Target:
                    result = Value.FromString(GetTargetPath());
                    break;
                case Property.Type.Name:
                    result = Value.FromString(Item.Name);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        public void SetProperty(Property.Type property, Value val)
        {
            switch (property)
            {
                case Property.Type.Visible:
                    Item.Visible = val.ToBoolean();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Calculates the absolute target path
        /// </summary>
        /// <returns>the target</returns>
        private string GetTargetPath()
        {
            string path;

            if (GetParent() == null)
                path = "/";
            else
            {
                path = GetParent().GetTargetPath();
                path += Item.Name;
            }

            return path;
        }
    }
}
