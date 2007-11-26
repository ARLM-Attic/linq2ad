/*
 * LINQ to Active Directory
 * http://www.codeplex.com/LINQtoAD
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoAD/Project/License.aspx for more information.
 */

#region Namespace imports

using System;

#endregion

namespace BdsSoft.DirectoryServices.Linq
{
    /// <summary>
    /// Specifies the directory schema to query.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DirectorySchemaAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Creates a new schema indicator attribute.
        /// </summary>
        /// <param name="schema">Name of the schema to query for.</param>
        public DirectorySchemaAttribute(string schema)
        {
            Schema = schema;
        }

        /// <summary>
        /// Creates a new schema indicator attribute.
        /// </summary>
        /// <param name="schema">Name of the schema to query for.</param>
        /// <param name="activeDsHelperType">Helper type for Active DS object properties.</param>
        public DirectorySchemaAttribute(string schema, Type activeDsHelperType)
        {
            Schema = schema;
            ActiveDsHelperType = activeDsHelperType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the schema to query for.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Helper type for Active DS object properties.
        /// </summary>
        public Type ActiveDsHelperType { get; set; }

        #endregion
    }

    /// <summary>
    /// Specifies the underlying attribute to query for in the directory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DirectoryAttributeAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Creates a new attribute binding attribute for a entity class field or property.
        /// </summary>
        /// <param name="attribute">Name of the attribute to query for.</param>
        public DirectoryAttributeAttribute(string attribute)
        {
            Attribute = attribute;
            Type = DirectoryAttributeType.Ldap;
        }

        /// <summary>
        /// Creates a new attribute binding attribute for a entity class field or property.
        /// </summary>
        /// <param name="attribute">Name of the attribute to query for.</param>
        /// <param name="type">Type of the underlying query source to get the attribute from.</param>
        public DirectoryAttributeAttribute(string attribute, DirectoryAttributeType type)
        {
            Attribute = attribute;
            Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the attribute to query for.
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Type of the underlying query source to get the attribute from.
        /// </summary>
        public DirectoryAttributeType Type { get; set; }

        #endregion
    }

    /// <summary>
    /// Type of the query source to perform queries with.
    /// </summary>
    public enum DirectoryAttributeType
    {
        /// <summary>
        /// Default value. Uses the Properties collection of DirectoryEntry to get data from.
        /// </summary>
        Ldap,

        /// <summary>
        /// Uses Active DS Helper IADs* objects to get data from.
        /// </summary>
        ActiveDs
    }
}