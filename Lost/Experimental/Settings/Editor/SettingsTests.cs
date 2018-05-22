//-----------------------------------------------------------------------
// <copyright file="SettingsTests.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    //using System;
    //using System.Collections;
    ////using NUnit.Framework;
    //using UnityEngine;
    //
    //internal class TestSettingsClass : Settings
    //{
    //    public IBoolSetting BoolSetting { get; private set; }
    //    public IIntSetting IntSetting { get; private set; }
    //    public IFloatSetting FloatSetting { get; private set; }
    //    public IStringSetting StringSetting { get; private set; }
    //    public IDateTimeSetting DateTimeSetting { get; private set; }
    //
    //    internal TestSettingsClass(string json) : base(json)
    //    {
    //        this.Initialize();
    //    }
    //
    //    internal TestSettingsClass() : base(null)
    //    {
    //        this.Initialize();
    //    }
    //
    //    internal void Initialize()
    //    {
    //        BoolSetting = GetBoolSetting(0, true);
    //        IntSetting = GetIntSetting(0, -1);
    //        FloatSetting = GetFloatSetting(0, -100.0f);
    //        StringSetting = GetStringSetting(0, "Default");
    //        DateTimeSetting = GetDateTimeSetting(0, DateTime.MinValue);
    //    }
    //}
    //
    //[TestFixture]
    //[Category("Settings Tests")]
    //internal class SettingsTests
    //{
    //    [Test]
    //    [Category("Default Values")]
    //    public void DefaultValuesTest()
    //    {
    //        var testSettings = new TestSettingsClass();
    //        Assert.AreEqual(testSettings.BoolSetting.Value, true);
    //        Assert.AreEqual(testSettings.IntSetting.Value, -1);
    //        Assert.AreEqual(testSettings.FloatSetting.Value, -100.0f);
    //        Assert.AreEqual(testSettings.StringSetting.Value, "Default");
    //        Assert.AreEqual(testSettings.DateTimeSetting.Value, DateTime.MinValue);
    //    }
    //
    //    [Test]
    //    [Category("Setting Values")]
    //    public void SettingValuesTest()
    //    {
    //        var testSettings = new TestSettingsClass();
    //        testSettings.BoolSetting.Value = false;
    //        testSettings.IntSetting.Value = 50;
    //        testSettings.FloatSetting.Value = 9.0f;
    //        testSettings.StringSetting.Value = "New Value";
    //        testSettings.DateTimeSetting.Value = DateTime.MaxValue;
    //
    //        Assert.AreEqual(testSettings.BoolSetting.Value, false);
    //        Assert.AreEqual(testSettings.IntSetting.Value, 50);
    //        Assert.AreEqual(testSettings.FloatSetting.Value, 9.0f);
    //        Assert.AreEqual(testSettings.StringSetting.Value, "New Value");
    //        Assert.AreEqual(testSettings.DateTimeSetting.Value, DateTime.MaxValue);
    //    }
    //
    //    [Test]
    //    [Category("Reverting Values")]
    //    public void RevertValuesTest()
    //    {
    //        var testSettings = new TestSettingsClass();
    //        testSettings.BoolSetting.Value = false;
    //        testSettings.IntSetting.Value = 50;
    //        testSettings.FloatSetting.Value = 9.0f;
    //        testSettings.StringSetting.Value = "New Value";
    //        testSettings.DateTimeSetting.Value = DateTime.MaxValue;
    //
    //        Assert.True(testSettings.IsDirty);
    //        testSettings.Revert();
    //        Assert.False(testSettings.IsDirty);
    //
    //        Assert.AreEqual(testSettings.BoolSetting.Value, true);
    //        Assert.AreEqual(testSettings.IntSetting.Value, -1);
    //        Assert.AreEqual(testSettings.FloatSetting.Value, -100.0f);
    //        Assert.AreEqual(testSettings.StringSetting.Value, "Default");
    //        Assert.AreEqual(testSettings.DateTimeSetting.Value, DateTime.MinValue);
    //    }
    //
    //    [Test]
    //    [Category("Saving Values")]
    //    public void SavingValuesTest()
    //    {
    //        var testSettings = new TestSettingsClass();
    //        testSettings.BoolSetting.Value = false;
    //        testSettings.IntSetting.Value = 50;
    //        testSettings.FloatSetting.Value = 9.0f;
    //        testSettings.StringSetting.Value = "New Value";
    //        testSettings.DateTimeSetting.Value = DateTime.MaxValue;
    //
    //        Assert.True(testSettings.IsDirty);
    //        testSettings.Commit();
    //        Assert.False(testSettings.IsDirty);
    //
    //        Assert.AreEqual(testSettings.BoolSetting.Value, false);
    //        Assert.AreEqual(testSettings.IntSetting.Value, 50);
    //        Assert.AreEqual(testSettings.FloatSetting.Value, 9.0f);
    //        Assert.AreEqual(testSettings.StringSetting.Value, "New Value");
    //        Assert.AreEqual(testSettings.DateTimeSetting.Value, DateTime.MaxValue);
    //    }
    //
    //    [Test]
    //    [Category("Serialization")]
    //    public void SerializationTest()
    //    {
    //        DateTime now = DateTime.UtcNow;
    //
    //        var testSettings = new TestSettingsClass();
    //        testSettings.BoolSetting.Value = false;
    //        testSettings.IntSetting.Value = 50;
    //        testSettings.FloatSetting.Value = 9.0f;
    //        testSettings.StringSetting.Value = "New Value";
    //        testSettings.DateTimeSetting.Value = now;
    //
    //        string json = testSettings.SerializeToJson();
    //        testSettings = new TestSettingsClass(json);
    //
    //        Assert.AreEqual(testSettings.BoolSetting.Value, false);
    //        Assert.AreEqual(testSettings.IntSetting.Value, 50);
    //        Assert.AreEqual(testSettings.FloatSetting.Value, 9.0f);
    //        Assert.AreEqual(testSettings.StringSetting.Value, "New Value");
    //
    //        // have to do this test because serialization loses a couple ticks
    //        Assert.True(testSettings.DateTimeSetting.Value.IsEqualTo(now));
    //    }
    //
    //    [Test]
    //    [Category("IsDirty On Change")]
    //    public void IsDirtyOnChangeTest()
    //    {
    //        TestSettingsClass testSettings = new TestSettingsClass();
    //        Assert.False(testSettings.IsDirty);
    //
    //        testSettings = new TestSettingsClass();
    //        testSettings.BoolSetting.Value = false;
    //        Assert.True(testSettings.IsDirty);
    //
    //        testSettings = new TestSettingsClass();
    //        testSettings.IntSetting.Value = 50;
    //        Assert.True(testSettings.IsDirty);
    //
    //        testSettings = new TestSettingsClass();
    //        testSettings.FloatSetting.Value = 9.0f;
    //        Assert.True(testSettings.IsDirty);
    //
    //        testSettings = new TestSettingsClass();
    //        testSettings.StringSetting.Value = "New Value";
    //        Assert.True(testSettings.IsDirty);
    //
    //        testSettings = new TestSettingsClass();
    //        testSettings.DateTimeSetting.Value = DateTime.MaxValue;
    //        Assert.True(testSettings.IsDirty);
    //    }
    //
    //    [Test]
    //    [Category("IsDirty On No Change")]
    //    public void IsDirtyOnNoChangeTest()
    //    {
    //        TestSettingsClass testSettings = new TestSettingsClass();
    //        Assert.False(testSettings.IsDirty);
    //
    //        testSettings.BoolSetting.Value = true;
    //        Assert.False(testSettings.IsDirty);
    //
    //        testSettings.IntSetting.Value = -1;
    //        Assert.False(testSettings.IsDirty);
    //
    //        testSettings.FloatSetting.Value = -100.0f;
    //        Assert.False(testSettings.IsDirty);
    //
    //        testSettings.StringSetting.Value = "Default";
    //        Assert.False(testSettings.IsDirty);
    //
    //        testSettings.DateTimeSetting.Value = DateTime.MinValue;
    //        Assert.False(testSettings.IsDirty);
    //    }
    //}
}
