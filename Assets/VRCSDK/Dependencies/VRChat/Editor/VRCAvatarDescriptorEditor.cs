/*
 *                                  VRC to CVR converter script 
 *                                  
 *                          made by Zettai Ryouiki with help from knah.
 * 
 *                  Anything can break, make backups if you don't have any before you use this script.
 *
 *  Copyright 2020, Zettai Ryouiki
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(VRCSDK2.VRC_AvatarDescriptor))]
public class AvatarDescriptorEditor : Editor
{
    VRCSDK2.VRC_AvatarDescriptor avatarDescriptor;
    VRC.Core.PipelineManager pipelineManager;

    SkinnedMeshRenderer selectedMesh;
    List<string> blendShapeNames = null;
    List<string> visemeNames = null;

    private readonly List<(string, string)> animationNameMatching_CVRFromVRC_Base = new List<(string, string)>
    {
        ("HandLeftFist","FIST"),
        ("HandRightFist","FIST"),
        ("HandLeftGun","HANDGUN"),
        ("HandRightGun","HANDGUN"),
        ("HandLeftOpen","HANDOPEN"),
        ("HandRightOpen","HANDOPEN"),
        ("HandLeftPeace","VICTORY"),
        ("HandRightPeace","VICTORY"),
        ("HandRightPoint","FINGERPOINT"),
        ("HandLeftPoint","FINGERPOINT"),
        ("HandLeftRocknroll","ROCKNROLL"),
        ("HandRightRocknroll","ROCKNROLL"),
        ("HandRightThumbsup","THUMBSUP"),
        ("HandLeftThumbsup","THUMBSUP"),
        ("Emote3","EMOTE3"),
        ("Emote4","EMOTE4"),
        ("Emote1","EMOTE1"),
        ("Emote2","EMOTE2"),
        ("Emote5","EMOTE5"),
        ("Emote6","EMOTE6"),
        ("Emote7","EMOTE7"),
        ("Emote8","EMOTE8"),
        ("LocProneBackward","PRONEFWD"),
        ("LocWalkingForward","WALKFWD"),
        ("LocWalkingBackwards","WALKBACK"),
        ("LocRunningStrafeRight","RUNSTRAFERT45"),
        ("LocRunningStrafeLeft","RUNSTRAFELT45"),
        ("LocRunningForward","RUNFWD"),
        ("LocRunningBackward","RUNBACK"),
        ("LocWalkingStrafeRight","STRAFERT"),
        ("LocCrouchIdle","CROUCHIDLE"),
        ("LocProneForward","PRONEFWD"),
        ("LocProneIdle","PRONEIDLE"),
        ("LocProneLeft","PRONEFWD"),
        ("LocProneRight","PRONEFWD"),
        ("LocCrouchRight","CROUCHWALKRT"),
        ("LocCrouchForward","CROUCHWALKFWD"),
        ("LocCrouchBackward","CROUCHWALKFWD"),
        ("LocJumpAir","FALL"),
    };
    private readonly (string, string) animationNameMatching_CVRFromVRC_Standing = ("LocIdle","IDLE");
    private readonly (string, string) animationNameMatching_CVRFromVRC_Sitting = ("LocSitting", "IDLE");
    private readonly List<(string, string)> animationNameMatching_CVRFromVRC_ToMirror = new List<(string, string)>
    {
        ("LocCrouchLeft","CROUCHWALKRT"),
        ("LocWalkingStrafeLeft","STRAFERT"),
    };
    
    private AnimatorOverrideController ConvertOverrideController(AnimatorOverrideController CVRController, AnimatorOverrideController VRCControllerStand, AnimatorOverrideController VRCControllerSit)
    {
        if (VRCControllerStand == null && VRCControllerSit == null) 
        {
            return CVRController;
        }
        var VCROverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        var VCROverridesSit = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        var CVROverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        string name = "";
        if (VRCControllerSit != null) 
        {
            VRCControllerSit.GetOverrides(VCROverridesSit);
            name = AssetDatabase.GetAssetPath(VRCControllerSit.GetInstanceID());
        }
        if (VRCControllerStand != null) 
        {
            VRCControllerStand.GetOverrides(VCROverrides);
            name = AssetDatabase.GetAssetPath(VRCControllerStand.GetInstanceID());
        }
        else { VCROverrides = VCROverridesSit; }
        
        CVRController.GetOverrides(CVROverrides);
        int indexVRC = 0;
        int indexCVR = 0;
        bool useSittingIdle = false;
        var animationNameMatchingCVRFromVRC = animationNameMatching_CVRFromVRC_Base;

        if (VRCControllerStand != null && VRCControllerSit != null)
        {
            if (VRCControllerStand.GetInstanceID().Equals(VRCControllerSit.GetInstanceID()))
            {
                animationNameMatchingCVRFromVRC.Add(animationNameMatching_CVRFromVRC_Standing);
            }
            else 
            {
                useSittingIdle = true;
            }
        }
        else
        {
            if (VRCControllerStand != null)
            {
                animationNameMatchingCVRFromVRC.Add(animationNameMatching_CVRFromVRC_Standing);
            }
            else if (VRCControllerSit != null)
            {
                useSittingIdle = true;
            }
        }

        foreach (var animationPair in animationNameMatchingCVRFromVRC) 
        {
            indexVRC = VCROverrides.FindIndex(x => x.Key.name.Equals(animationPair.Item2));
            indexCVR = CVROverrides.FindIndex(x => x.Key.name.Equals(animationPair.Item1));
            try
            {
                if (indexVRC >= 0 && indexCVR >= 0 && VCROverrides[indexVRC].Value != null)
                {
                    CVROverrides[indexCVR] = new KeyValuePair<AnimationClip, AnimationClip>(CVROverrides[indexCVR].Key, VCROverrides[indexVRC].Value);
                }
            }
            catch (Exception e) { Debug.LogWarning(e.Message); }
        }
        foreach (var animationPair in animationNameMatching_CVRFromVRC_ToMirror)
        {
            //some animations are mirrored in the controller in VRC, but separate entries in CVR
            indexVRC = VCROverrides.FindIndex(x => x.Key.name.Equals(animationPair.Item2));
            indexCVR = CVROverrides.FindIndex(x => x.Key.name.Equals(animationPair.Item1));
            try
            {
                if (indexVRC >= 0 && indexCVR >= 0 && VCROverrides[indexVRC].Value != null)
                {
                    string path = AssetDatabase.GetAssetPath(VCROverrides[indexVRC].Value);
                    string newPath = path.EndsWith(".anim") ? path.Remove(path.LastIndexOf(".anim")) + "_mirror.anim" : path + "_mirror.anim";
                    AssetDatabase.CopyAsset(path, newPath);
                    var mirroredAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(newPath);
                    var animationClipSettings = AnimationUtility.GetAnimationClipSettings(mirroredAnim);
                    animationClipSettings.mirror = !animationClipSettings.mirror;
                    AnimationUtility.SetAnimationClipSettings(mirroredAnim, animationClipSettings);
                    CVROverrides[indexCVR] = new KeyValuePair<AnimationClip, AnimationClip>(CVROverrides[indexCVR].Key, mirroredAnim);
                }
            }
            catch (Exception e) { Debug.LogWarning(e.Message); }
            
        }
        if (useSittingIdle) 
        {
            indexVRC = VCROverridesSit.FindIndex(x => x.Key.name.Equals(animationNameMatching_CVRFromVRC_Sitting.Item2));
            indexCVR = CVROverrides.FindIndex(x => x.Key.name.Equals(animationNameMatching_CVRFromVRC_Sitting.Item1));
            try
            {
                CVROverrides[indexCVR] = new KeyValuePair<AnimationClip, AnimationClip>(CVROverrides[indexCVR].Key, VCROverrides[indexVRC].Value);
            }
            catch (Exception e) { Debug.LogWarning(e.Message); }
        }
        CVRController.name = VRCControllerStand.name + "_CVR";
        CVRController.ApplyOverrides(CVROverrides);
        string assetName = name.EndsWith(".overrideController") ? name.Remove(name.LastIndexOf(".overrideController")) + "_CVR.overrideController" : name + "_CVR.overrideController";
        //let's just overwrite the file if it exists
        AssetDatabase.CreateAsset(CVRController, assetName);
        return CVRController;
    }
    private void ConvertToCVRAvatar() 
    {
        var start = DateTime.Now;
        avatarDescriptor.gameObject.TryGetComponent(out ABI.CCK.Components.CVRAvatar avatar);
        if (avatar == null)
        {
            avatar = avatarDescriptor.gameObject.AddComponent<ABI.CCK.Components.CVRAvatar>();
        }
        if (avatarDescriptor.CustomSittingAnims != null || avatarDescriptor.CustomStandingAnims != null)
        {
            // create new ovc is there's none there already
            //default path is Assets/ABI.CCK/Animations/AvatarAnimator.controller GUID ff926e022d914b84e8975ba6188a26f0
            if (avatar.overrides == null)
            {
                //if both path and GUID are broken then fix it yourself.
                var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/ABI.CCK/Animations/AvatarAnimator.controller");
                if (controller == null) { controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AssetDatabase.GUIDToAssetPath("ff926e022d914b84e8975ba6188a26f0")); }
                AnimatorOverrideController newController = new AnimatorOverrideController(controller);
                newController = ConvertOverrideController(newController, avatarDescriptor.CustomStandingAnims, avatarDescriptor.CustomSittingAnims);
                avatar.overrides = newController;
            }
        }
        // if lipsync is set to viseme, copy the mesh and the blendshapes
        if (avatarDescriptor.lipSync == VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape)
        {
            if (avatarDescriptor.VisemeSkinnedMesh != null)
            {
                avatar.bodyMesh = avatarDescriptor.VisemeSkinnedMesh;
                if (avatar.bodyMesh.sharedMesh.blendShapeCount > 4)
                {
                    //vrc uses the first 4 blendshapes for blinking. if it looks broken then it was broken in vrc too.
                    avatar.blinkBlendshape = new string[]
                    {
                                avatar.bodyMesh.sharedMesh.GetBlendShapeName(0),
                                avatar.bodyMesh.sharedMesh.GetBlendShapeName(1),
                                avatar.bodyMesh.sharedMesh.GetBlendShapeName(2),
                                avatar.bodyMesh.sharedMesh.GetBlendShapeName(3)
                    };
                    avatar.useBlinkBlendshapes = true;
                }
            }
            if (avatarDescriptor.VisemeBlendShapes != null && avatarDescriptor.VisemeBlendShapes.Length > 0)
            {
                avatar.useVisemeLipsync = true;
                //cvr order is the same as vrc order
                avatar.visemeBlendshapes = avatarDescriptor.VisemeBlendShapes;
            }
        }
        else if (avatarDescriptor.lipSync == VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.JawFlapBlendShape)
        {
            //set the body mesh
            if (avatarDescriptor.VisemeSkinnedMesh != null)
            {
                avatar.bodyMesh = avatarDescriptor.VisemeSkinnedMesh;
            }
            avatar.useVisemeLipsync = false;
        }
        //copy view position
        avatar.viewPosition = avatarDescriptor.ViewPosition;
        //guess voice position from 'oh' blendshape
        if (avatar.useVisemeLipsync)
        {
            float smoothing = 0.001f * selectedMesh.sharedMesh.bounds.max.magnitude; //the amount of minimum vert movement when we consider it moving the mouth. too low and it might get inaccurate results, too high and it fails to find any verts moving at all.
            int index = selectedMesh.sharedMesh.GetBlendShapeIndex(avatar.visemeBlendshapes[avatar.visemeBlendshapes.Length - 1]);
            var ohBlendVerts = new Vector3[selectedMesh.sharedMesh.vertexCount];
            var ohBlendNormals = new Vector3[selectedMesh.sharedMesh.vertexCount];
            var ohBlendTangents = new Vector3[selectedMesh.sharedMesh.vertexCount]; // Copy blend shape data from myMesh to tmpMesh
            var blendVerts = new Vector3[selectedMesh.sharedMesh.vertexCount];
            var meshVerts = selectedMesh.sharedMesh.vertices; //caching this makes it literally 100x faster. 
            var rootScale = selectedMesh.rootBone.transform.lossyScale.magnitude; //scale with armature scale, ie. when Armature is scaled at magnitude 100 instead of 1.
            int j = 0; //moving vert counter
            try
            {
                selectedMesh.sharedMesh.GetBlendShapeFrameVertices(index, selectedMesh.sharedMesh.GetBlendShapeFrameCount(index) - 1, ohBlendVerts, ohBlendNormals, ohBlendTangents);
                for (int i = 0; i < ohBlendVerts.Length; i++)
                {
                    if (ohBlendVerts[i].magnitude > smoothing / rootScale)
                    {
                        blendVerts[j] = meshVerts[i];
                        j++;
                    }
                }
                var ohverts = new Vector3[j];
                for (int i = 0; i < j; i++)
                {
                    ohverts[i] = blendVerts[i];
                }

                var posAverage = new Vector3(
                    ohverts.Average(x => x.x),
                    ohverts.Average(x => x.y),
                    ohverts.Average(x => x.z)
                    );
                Debug.Log($"mesh vert count: {meshVerts.Length}, blendshape vert count: {j} ({Mathf.Round(j * 10000f / meshVerts.Length) / 100f}%), raw position: {posAverage}");
                var voicePoint = selectedMesh.transform.TransformPoint(posAverage) - avatar.transform.position;
                avatar.voicePosition = new Vector3(
                    (Mathf.Round(voicePoint.x * 1000f) / 1000f),
                    (Mathf.Round(voicePoint.y * 1000f) / 1000f),
                    (Mathf.Round(voicePoint.z * 1000f) / 1000f)
                    );
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        else
        {
            //when we don't have visemes guess voicePosition from viewPosition
            avatar.voicePosition = new Vector3
            {
                x = (Mathf.Round(avatar.viewPosition.x * 1000f) / 1000f),
                y = (Mathf.Round(avatar.viewPosition.y / 1.05f * 1000f) / 1000f), //this is usually fairly close
                z = (Mathf.Round(avatar.viewPosition.z * 1.7f * 1000f) / 1000f)   //this can be off
            };
        }
        EditorUtility.SetDirty(avatarDescriptor.gameObject);

        var end = DateTime.Now;
        var elapsed = (end - start).TotalMilliseconds;
        Debug.Log($"Conversion took {elapsed} ms.");
    }
    public override void OnInspectorGUI()
    {
        if (avatarDescriptor == null)
            avatarDescriptor = (VRCSDK2.VRC_AvatarDescriptor)target;

        if (GUILayout.Button("Convert to CVR Avatar Descriptor!"))
        {
            ConvertToCVRAvatar();
        }
        avatarDescriptor.ViewPosition = EditorGUILayout.Vector3Field("View Position", avatarDescriptor.ViewPosition);
        avatarDescriptor.Animations = (VRCSDK2.VRC_AvatarDescriptor.AnimationSet)EditorGUILayout.EnumPopup("Default Animation Set", avatarDescriptor.Animations);
        avatarDescriptor.CustomStandingAnims = (AnimatorOverrideController)EditorGUILayout.ObjectField("Custom Standing Animations", avatarDescriptor.CustomStandingAnims, typeof(AnimatorOverrideController), true, null);
        avatarDescriptor.CustomSittingAnims = (AnimatorOverrideController)EditorGUILayout.ObjectField("Custom Sitting Animations", avatarDescriptor.CustomSittingAnims, typeof(AnimatorOverrideController), true, null);
        //avatarDescriptor.ScaleIPD doesn't do anything
        avatarDescriptor.lipSync = (VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle)EditorGUILayout.EnumPopup("Lip Sync", avatarDescriptor.lipSync);
        int next;
        switch (avatarDescriptor.lipSync)
        {
            case VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.Default:
                break;

            case VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.JawFlapBlendShape:
                avatarDescriptor.VisemeSkinnedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Face Mesh", avatarDescriptor.VisemeSkinnedMesh, typeof(SkinnedMeshRenderer), true);
                if (avatarDescriptor.VisemeSkinnedMesh != null)
                {
                    DetermineBlendShapeNames();

                    int current = blendShapeNames.FindIndex(x => x.Equals(avatarDescriptor.MouthOpenBlendShapeName));

                    string title = "Jaw Flap Blend Shape";
                    if ((next = EditorGUILayout.Popup(title, current, blendShapeNames.ToArray())) >= 0) 
                    {
                        avatarDescriptor.MouthOpenBlendShapeName = blendShapeNames[next];
                        EditorUtility.SetDirty(target);
                    }
                }
                break;

            case VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.JawFlapBone:
                avatarDescriptor.lipSyncJawBone = (Transform)EditorGUILayout.ObjectField("Jaw Bone", avatarDescriptor.lipSyncJawBone, typeof(Transform), true);
                break;

            case VRCSDK2.VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape:
                avatarDescriptor.VisemeSkinnedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Face Mesh", avatarDescriptor.VisemeSkinnedMesh, typeof(SkinnedMeshRenderer), true);
                if (avatarDescriptor.VisemeSkinnedMesh != null)
                {
					DetermineBlendShapeNames();
					FillVisemeNames();
                    int visemeCount = (int)VRCSDK2.VRC_AvatarDescriptor.Viseme.Count;

                    if (avatarDescriptor.VisemeBlendShapes == null || avatarDescriptor.VisemeBlendShapes.Length != visemeCount)
                    {
                        avatarDescriptor.VisemeBlendShapes = new string[visemeCount];
                        FillVisemeArray(visemeNames.Count, visemeNames.ToArray());
                    }
                    else
                    {
                        FillVisemeArray(visemeCount, avatarDescriptor.VisemeBlendShapes);
                    }
                }
                break;
        }
        if (GUILayout.Button("Remove VRC avatar components"))
        {
            if (EditorUtility.DisplayDialog("Remove VRC components", "Are you sure you want to remove the VRC avatar descriptor from this avatar?", "Yes", "No"))
            {
                GameObject _avatar = avatarDescriptor.gameObject;
                RemoveVRCAvatarComponents(_avatar);
                pipelineManager = avatarDescriptor.gameObject.GetComponent<VRC.Core.PipelineManager>();
                DestroyImmediate(avatarDescriptor);
                if(pipelineManager != null) DestroyImmediate(pipelineManager);
                EditorUtility.SetDirty(_avatar);
                return;
            }
        }
        EditorGUILayout.LabelField("Unity Version", avatarDescriptor.unityVersion);
    }

    private void RemoveVRCAvatarComponents(GameObject avatar)
    {
        var IKFollowers = avatar.GetComponentsInChildren<VRCSDK2.VRC_IKFollower>(true);
        var VRCStations = avatar.GetComponentsInChildren<VRCSDK2.VRC_Station>(true);
        foreach (var component in IKFollowers) 
        {
            DestroyImmediate(component);
        }
        foreach (var component in VRCStations)
        {
            DestroyImmediate(component);
        }
    }

    void FillVisemeArray(int visemeCount, string[] visemes) 
    {
        int next;
        for (int i = 0; i < visemeCount; ++i)
        {
            int current = blendShapeNames.FindIndex(x => x.StartsWith(visemes[i]));
            string title = $"Viseme: {(VRCSDK2.VRC_AvatarDescriptor.Viseme)i}";
            if ((next = EditorGUILayout.Popup(title, current, blendShapeNames.ToArray())) >= 0)
            {
                avatarDescriptor.VisemeBlendShapes[i] = blendShapeNames[next];
                EditorUtility.SetDirty(target);
            }
        }
    }
    void DetermineBlendShapeNames()
    {
        if (avatarDescriptor.VisemeSkinnedMesh != null && avatarDescriptor.VisemeSkinnedMesh != selectedMesh)
        {
            selectedMesh = avatarDescriptor.VisemeSkinnedMesh;
            Mesh m = selectedMesh.sharedMesh;
            blendShapeNames = new List<string> { "-none-" };
            for (int i = 1; i < m.blendShapeCount; i++) { blendShapeNames.Add(m.GetBlendShapeName(i)); }
        }
    }

    void FillVisemeNames()
    {
        visemeNames = new List<string>
        {
            "vrc.v_sil",
            "vrc.v_pp",
            "vrc.v_ff",
            "vrc.v_th",
            "vrc.v_dd",
            "vrc.v_kk",
            "vrc.v_ch",
            "vrc.v_ss",
            "vrc.v_nn",
            "vrc.v_rr",
            "vrc.v_aa",
            "vrc.v_e",
            "vrc.v_ih",
            "vrc.v_oh",
            "vrc.v_ou"
        };
    }
}
