using FirstAlert.Enums;
using Org.Tensorflow;
using Org.Tensorflow.Contrib.Android;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Util;

namespace FirstAlert.Algorithm
{
    public class ImageClassifier
    {
        private static readonly string TAG = "CameraFragment";

        private const string PB_FILE = "models.pb";
        private const string LABEL_FILE = "labels.txt";
        
        private List<string> _labels;
        TensorFlowInferenceInterface _tfinterface;

        private bool _hasNormalizationLayer;
        private ModelType _modelType;

        private static int InputSize = 224;
        private const string InputName = "Placeholder";
        private const string OutputName = "Softmax";

        private const string DataNormLayerPrefix = "data_bn";

        public ImageClassifier()
        {
            try
            {
                var assets = Android.App.Application.Context.Assets;
                _tfinterface = new TensorFlowInferenceInterface(assets, PB_FILE);

                using var sr = new StreamReader(assets.Open(LABEL_FILE));
                var content = sr.ReadToEnd();
                _labels = content.Split("\n")
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();

                InputSize = Convert.ToInt32(_tfinterface.GraphOperation(InputName).Output(0).Shape().Size(1));
                var iter = _tfinterface.Graph().Operations();
                while (iter.HasNext && !_hasNormalizationLayer)
                {
                    var op = iter.Next() as Operation;
                    if (op.Name().Contains(DataNormLayerPrefix))
                    {
                        _hasNormalizationLayer = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.ToString());
            }
        }
    }
}