//-----------------------------------------------------------------------
// <copyright file="ProjectSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public enum LineEndings
    {
        Windows,
        Unix
    }

    [Serializable]
    public class ProjectSettingsData
    {
        [Header("P4Ignore")]
        [SerializeField] private string p4IgnoreFileName = ".p4ignore";
        [SerializeField] private bool setP4IgnoreFlagAtStartup = true;
        [SerializeField] private bool generateP4IgnoreIfDoesNotExist = true;
        [SerializeField] private string templateP4IgnoreLocation = "./Assets/Code/Lost/Editor/ProjectSettings/DefaultFiles/p4ignore.txt";

        [Header("Line Endings")]
        [SerializeField] private LineEndings projectLineEndings = LineEndings.Windows;
        
        [Header("Template Files")]
        [SerializeField] private bool overrideTemplateCShardFiles = true;
        [SerializeField] private string templateNewBehaviourLocation = "./Assets/Code/Lost/Editor/ProjectSettings/DefaultFiles/NewBehaviourScript.cs.txt";
        [SerializeField] private string templateNewEditorTestLocation = "./Assets/Code/Lost/Editor/ProjectSettings/DefaultFiles/NewEditorTest.cs.txt";
        [SerializeField] private string templateNewStatMachineLocation = "./Assets/Code/Lost/Editor/ProjectSettings/DefaultFiles/NewStateMachineBehaviourScript.cs.txt";
        [SerializeField] private string templateNewSubStateMachineLocation = "./Assets/Code/Lost/Editor/ProjectSettings/DefaultFiles/NewSubStateMachineBehaviourScript.cs.txt";

        [Header("Project Defines")]
        [SerializeField] private bool warningsAsErrors = true;
        [SerializeField] private string definesList = "";

        public string P4IgnoreFileName
        {
            get { return this.p4IgnoreFileName; }
        }

        public bool SetP4IgnoreFlagAtStartup
        {
            get { return this.setP4IgnoreFlagAtStartup; }
        }

        public bool GenerateP4IgnoreIfDoesNotExist
        {
            get { return this.generateP4IgnoreIfDoesNotExist; }
        }

        public string TemplateP4IgnoreLocation
        {
            get { return this.templateP4IgnoreLocation; }
        }

        public LineEndings ProjectLineEndings
        {
            get { return this.projectLineEndings; }
        }

        public bool OverrideTemplateCShardFiles
        {
            get { return this.overrideTemplateCShardFiles; }
        }

        public string TemplateNewBehaviourLocation
        {
            get { return this.templateNewBehaviourLocation; }
        }

        public string TemplateNewEditorTestLocation
        {
            get { return this.templateNewEditorTestLocation; }
        }

        public string TemplateNewStatMachineLocation
        {
            get { return this.templateNewStatMachineLocation; }
        }

        public string TemplateNewSubStateMachineLocation
        {
            get { return this.templateNewSubStateMachineLocation; }
        }

        public bool WarningsAsErrors
        {
            get { return this.warningsAsErrors; }
        }

        public string DefinesList
        {
            get { return this.definesList; }
        }
    }
}
