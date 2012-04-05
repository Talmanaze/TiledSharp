﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;
using System.Reflection;

namespace TiledSharp
{
    public class Tileset : ITiledClass
    {
        public string Name {get; set;}
        
        public string source;
        public int? firstGid;       // Required for TMX, but not TSX
        public int? tileWidth, tileHeight;
        public int spacing = 0;
        public int margin = 0;
        
        public Image image;
        public PropertyDict property;
        public Dictionary<int, PropertyDict> tile;
        
        // Assumes one tileset entry per TSX file
        public Tileset(XDocument xDoc) : this(xDoc.Element("tileset")) { }
        
        public Tileset(XElement xTileset)
        {
            source = (string)xTileset.Attribute("source");
            firstGid = (int?)xTileset.Attribute("firstgid");            
            if (source == null)
            {
                Name = (string)xTileset.Attribute("name");
                image = new Image(xTileset.Element("image"));
                tileWidth = (int?)xTileset.Attribute("tilewidth");
                tileHeight = (int?)xTileset.Attribute("tileheight");
                
                var xSpacing = xTileset.Attribute("spacing");
                if (xSpacing != null)
                    spacing = (int)xSpacing;
                
                var xMargin = (int?)xTileset.Attribute("margin");
                if (xMargin != null)
                    margin = (int)xMargin;
                
                tile = new Dictionary<int, PropertyDict>();
                foreach (var xml_tile in xTileset.Elements("tile"))
                {
                    var id = (int)xml_tile.Attribute("id");
                    var xProp = xml_tile.Element("properties");
                    tile.Add(id, new PropertyDict(xProp));
                }
            }
            else
            {
                var xDocTileset = TiledIO.ReadXml(source);
                var ts = new Tileset(xDocTileset);
                Name = ts.Name;
                tileWidth = ts.tileWidth;
                tileHeight = ts.tileHeight;
                spacing = ts.spacing;
                margin = ts.margin;
                image = ts.image;
                tile = ts.tile;
            }            
        }
        
        public class Image
        {
            public string source;
            public uint? trans;  // 24-bit RGB transparent color
            
            public Image(XElement xImage)
            {
                source = (string)xImage.Attribute("source");
                
                var xml_trans = (string)xImage.Attribute("trans");
                if (xml_trans != null)
                    trans = UInt32.Parse(xml_trans, NumberStyles.HexNumber);
            }
        }
    }
}
