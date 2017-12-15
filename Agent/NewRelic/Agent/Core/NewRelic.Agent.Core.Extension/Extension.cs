// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.6.0.0
//    <NameSpace>NewRelic.Agent.Core.Extension</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><VirtualProp>False</VirtualProp><PascalCase>False</PascalCase><BaseClassName>EntityBase</BaseClassName><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net20</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>False</GenerateXMLAttributes><OrderXMLAttrib>False</OrderXMLAttrib><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>False</ExcludeIncludedTypes><InitializeFields>All</InitializeFields><GenerateAllTypes>True</GenerateAllTypes>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace NewRelic.Agent.Core.Extension
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Collections.Generic;
    
    
    public partial class extension
    {
        
        private List<extensionTracerFactory> instrumentationField;
        
        private bool enabledField;
        
        public extension()
        {
            this.instrumentationField = new List<extensionTracerFactory>();
            this.enabledField = true;
        }
        
        [System.Xml.Serialization.XmlArrayItemAttribute("tracerFactory", IsNullable=false)]
        public List<extensionTracerFactory> instrumentation
        {
            get
            {
                return this.instrumentationField;
            }
            set
            {
                this.instrumentationField = value;
            }
        }
        
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }
    }
    
    public partial class extensionTracerFactory
    {
        
        private List<extensionTracerFactoryMatch> matchField;
        
        private string nameField;
        
        private string metricNameField;
        
        private bool enabledField;
        
        private string levelField;
        
        private bool suppressRecursiveCallsField;
        
        private extensionTracerFactoryMetric metricField;
        
        private bool transactionTraceSegmentField;
        
        private System.Nullable<ushort> transactionNamingPriorityField;
        
        public extensionTracerFactory()
        {
            this.matchField = new List<extensionTracerFactoryMatch>();
            this.nameField = "NewRelic.Agent.Core.Tracer.Factories.DefaultTracerFactory";
            this.enabledField = true;
            this.levelField = "0";
            this.suppressRecursiveCallsField = true;
            this.metricField = extensionTracerFactoryMetric.scoped;
            this.transactionTraceSegmentField = true;
        }
        
        public List<extensionTracerFactoryMatch> match
        {
            get
            {
                return this.matchField;
            }
            set
            {
                this.matchField = value;
            }
        }
        
        [System.ComponentModel.DefaultValueAttribute("NewRelic.Agent.Core.Tracer.Factories.DefaultTracerFactory")]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        public string metricName
        {
            get
            {
                return this.metricNameField;
            }
            set
            {
                this.metricNameField = value;
            }
        }
        
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }
        
        [System.ComponentModel.DefaultValueAttribute("0")]
        public string level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }
        
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool suppressRecursiveCalls
        {
            get
            {
                return this.suppressRecursiveCallsField;
            }
            set
            {
                this.suppressRecursiveCallsField = value;
            }
        }
        
        [System.ComponentModel.DefaultValueAttribute(extensionTracerFactoryMetric.scoped)]
        public extensionTracerFactoryMetric metric
        {
            get
            {
                return this.metricField;
            }
            set
            {
                this.metricField = value;
            }
        }
        
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool transactionTraceSegment
        {
            get
            {
                return this.transactionTraceSegmentField;
            }
            set
            {
                this.transactionTraceSegmentField = value;
            }
        }
        
        public ushort transactionNamingPriority
        {
            get
            {
                if (this.transactionNamingPriorityField.HasValue)
                {
                    return this.transactionNamingPriorityField.Value;
                }
                else
                {
                    return default(ushort);
                }
            }
            set
            {
                this.transactionNamingPriorityField = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool transactionNamingPrioritySpecified
        {
            get
            {
                return this.transactionNamingPriorityField.HasValue;
            }
            set
            {
                if (value==false)
                {
                    this.transactionNamingPriorityField = null;
                }
            }
        }
    }
    
    public partial class extensionTracerFactoryMatch
    {
        
        private List<extensionTracerFactoryMatchExactMethodMatcher> exactMethodMatcherField;
        
        private string assemblyNameField;
        
        private string classNameField;
        
        public extensionTracerFactoryMatch()
        {
            this.exactMethodMatcherField = new List<extensionTracerFactoryMatchExactMethodMatcher>();
        }
        
        public List<extensionTracerFactoryMatchExactMethodMatcher> exactMethodMatcher
        {
            get
            {
                return this.exactMethodMatcherField;
            }
            set
            {
                this.exactMethodMatcherField = value;
            }
        }
        
        public string assemblyName
        {
            get
            {
                return this.assemblyNameField;
            }
            set
            {
                this.assemblyNameField = value;
            }
        }
        
        public string className
        {
            get
            {
                return this.classNameField;
            }
            set
            {
                this.classNameField = value;
            }
        }
    }
    
    public partial class extensionTracerFactoryMatchExactMethodMatcher
    {
        
        private string methodNameField;
        
        private string parametersField;
        
        public string methodName
        {
            get
            {
                return this.methodNameField;
            }
            set
            {
                this.methodNameField = value;
            }
        }
        
        public string parameters
        {
            get
            {
                return this.parametersField;
            }
            set
            {
                this.parametersField = value;
            }
        }
    }
    
    public enum extensionTracerFactoryMetric
    {
        
        /// <remarks/>
        none,
        
        /// <remarks/>
        scoped,
        
        /// <remarks/>
        unscoped,
        
        /// <remarks/>
        both,
    }
}
