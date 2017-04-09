using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace BOT_FrontEnd
{
    class VectorImage
    {
        private XmlDocument SVG_Source;

        private double XScaleFactor;
        private double YScaleFactor;

        private double[] limit_square;
        private double[] axis_offset;

        private String[] current_point = new String[2];

        public VectorImage(String SourcePath)
        {
            SVG_Source = new XmlDocument();
            String filetext = File.ReadAllText(SourcePath);
            axis_offset = new double[2];
            axis_offset[0] = 0;
            axis_offset[1] = 0;

            SVG_Source.LoadXml(filetext);
        }

        ~VectorImage()
        {
            
        }

        /********************************************************************************
         * FUNCTION:       setAxisOffset
         * Description:    
         * Parameters:  
         *      offset -   
         ********************************************************************************/
        public void setAxisOffset(double[] offset)
        {
            axis_offset[0] = offset[0];
            axis_offset[1] = offset[1];
        }

        /********************************************************************************
         * FUNCTION:       getCode
         * Description:    Manages parsing of the loaded SVG image. Returns the computed
         *                 G-code as a string.
         * Parameters:  
         *      scale -    defines the limits of the parsed G-code. 
         *      
         * ex. Assuming the local coordinate system of the receiver is defined in inches:
         *     scale = [4, 4] will parse the image into the commands required to draw it
         *     within a 4x4 inch square.
         ********************************************************************************/
        public String getCode(double[] scale)
        {
            String code = "";
            
            XmlElement svgNode = SVG_Source.DocumentElement;
            XmlAttributeCollection svgAttributes = svgNode.Attributes;
            double act_width, act_height;

            if(svgNode.GetAttribute("width") != String.Empty)
            {
                act_width = double.Parse(svgNode.GetAttribute("width").Replace("px", ""));
                act_height = double.Parse(svgNode.GetAttribute("height").Replace("px", ""));

                XScaleFactor = scale[0] / act_width;
                YScaleFactor = scale[1] / act_height;
            }
            else
            {
                XScaleFactor = 1.0;
                YScaleFactor = 1.0;
            }

            limit_square = new double[2];
            limit_square[0] = scale[0];
            limit_square[1] = scale[1];

            current_point[0] = String.Format("{0:0.0000}", axis_offset[0]);
            current_point[1] = String.Format("{0:0.0000}", axis_offset[1]);

            code = "\r\nG10;";
            foreach (XmlNode node in svgNode.ChildNodes)
            {
                if (node.Name == "g")
                {
                    foreach (XmlNode vect in node.ChildNodes)
                    {
                        code += parse_node(vect);
                    }
                }
            }
            code += "\r\nG11;";

            return code;
        }

        /********************************************************************************
         * FUNCTION:       parse_node
         * Description:    
         * Parameters:  
         *      node -   
         ********************************************************************************/
        private String parse_node(XmlNode node)
        {
            String parsed_txt = "";
            XmlNodeList node_list = node.ChildNodes;

            if (node_list.Count == 0)
            {
                if (node.Name.ToLower() == "line")
                {
                    parsed_txt = "\r\n(-- LINE --)" + line((XmlElement)node);
                }
                else if (node.Name.ToLower() == "path")
                {
                    parsed_txt = "\r\n(-- PATH --)" + path((XmlElement)node);
                }
                else if (node.Name.ToLower() == "polyline")
                {
                    parsed_txt = "\r\n(-- POLYLINE --)" + polyline((XmlElement)node);
                }
                else if (node.Name.ToLower() == "polygon")
                {
                    parsed_txt = "\r\n(-- POLYGON --)" + polygon((XmlElement)node);
                }
                else
                {
                    //Do Nothing
                }
            }
            else
            {
                foreach(XmlNode nested_node in node_list)
                {
                    parsed_txt += parse_node(nested_node);
                }
            }
            return parsed_txt;
        }

        /********************************************************************************
         * FUNCTION:        ScalePoint_X
         * Description:    
         * Parameters:  
         *      svgPoint -  Numeric value passed as string. The value to be scaled. 
         *      absolute -  Indicates whether the value is absolute (true) or relative (false)
         ********************************************************************************/
        private String ScalePoint_X(String svgPoint, bool absolute)
        {
            if (absolute == true)
                return String.Format("{0:0.0000}", (double.Parse(svgPoint) * XScaleFactor) + axis_offset[0]);
            else
                return String.Format("{0:0.0000}", (double.Parse(svgPoint) * XScaleFactor));
        }

        /********************************************************************************
         * FUNCTION:        ScalePoint_Y
         * Description:    
         * Parameters:  
         *      svgPoint -  Numeric value passed as string. The value to be scaled. 
         *      absolute -  Indicates whether the value is absolute (true) or relative (false)
         ********************************************************************************/
        private String ScalePoint_Y(String svgPoint, bool absolute)
        {
            if (absolute == true)
                return String.Format("{0:0.0000}", (limit_square[1] + axis_offset[1]) - (double.Parse(svgPoint) * YScaleFactor));
            else
                return String.Format("{0:0.0000}", -1*(double.Parse(svgPoint) * YScaleFactor));
        }

        /********************************************************************************
         * FUNCTION:        line
         * Description:     Parses an SVG 'line' element
         * Parameters:  
         *      source -    The SVG source
         ********************************************************************************/
        private String line(XmlElement source)
        {
            String code = "";
            String[] point = new String[2];

            point[0] = ScalePoint_X(source.GetAttribute("x2"), true);
            point[1] = ScalePoint_Y(source.GetAttribute("y2"), true);

            code += "\r\nG91Z-0.75;";
            code += "\r\nG90X" + ScalePoint_X(source.GetAttribute("x1"),true) +
                "Y" + ScalePoint_Y(source.GetAttribute("y1"), true) + ";";
            code += "\r\nG91Z0.75;";
            code += "\r\nG90X" + point[0] + "Y" + point[1] + ";";

            current_point[0] = String.Format("{0:0.0000}", point[0]);
            current_point[1] = String.Format("{0:0.0000}", point[1]);

            return code;
        }

        /********************************************************************************
         * FUNCTION:        polyline
         * Description:     Parses an SVG 'polyline' element
         * Parameters:  
         *      source -    The SVG source
         ********************************************************************************/
        private String polyline(XmlElement source)
        {
            String code = "";
            String src = source.GetAttribute("points").Replace("\r","").Replace("\n","").Replace("\t","");
            String[] points_str = src.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            String[] point = new String[2];
      
            int iter = 0;

            code += "\r\nG91Z-0.75;";
            foreach (string strpoint in points_str)
            {
                iter++;
                point = strpoint.Split(new char[] { ',' });

                code += "\r\nG90X" + ScalePoint_X(point[0], true) + "Y" + ScalePoint_Y(point[1], true) + ";";

                if (iter == 1)
                {
                    code += "\r\nG91Z0.75;";
                }
            }

            current_point[0] = String.Format("{0:0.0000}", ScalePoint_X(point[0], true));
            current_point[1] = String.Format("{0:0.0000}", ScalePoint_Y(point[1], true));
            
            return code;
        }

        /********************************************************************************
         * FUNCTION:        polygon
         * Description:     Parses an SVG 'polygon' element
         * Parameters:  
         *      source -    the SVG source
         ********************************************************************************/
        private String polygon(XmlElement source)
        {
            String code = "";
            String src = source.GetAttribute("points").Replace("\r", "").Replace("\n", "").Replace("\t", "");
            String[] points_str = src.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            String[] point = new String[2];

            code = polyline(source);
            point = points_str[0].Split(new char[] { ',' });

            code += "\r\nG90X" + ScalePoint_X(point[0], true) + "Y" + ScalePoint_Y(point[1], true) + ";";

            current_point[0] = String.Format("{0:0.0000}", ScalePoint_X(point[0], true));
            current_point[1] = String.Format("{0:0.0000}", ScalePoint_Y(point[1], true));
                    
            return code;
        }

        /********************************************************************************
         * FUNCTION:        path
         * Description:     Parses an SVG 'path' element
         * Parameters:  
         *      source -    The SVG source, passed as an XML element
         ********************************************************************************/
        private String path(XmlElement source)
        {
            String code = "", hold = "", cmd = "", temp = "";
            String[] point, point_list;
            double[,] curve_points;
            String src = source.GetAttribute("d");
            int index = 0, bi = 0, ci = 0, mi = 0;
            int num_points;
            double t = 0;
            char[] buffer = new char[16];
            double[] Bezier_Pt = new double[2];

            //Replace special characters 
            src = src.Replace("\r\n", String.Empty).Replace("\t", String.Empty);

            char[] delimiters = new char[] {'M','L','H','V','C','S','Q','T','A','Z',
                                            'm','l','h','v','c','s','q','t','a','z'};

            String[] points_str = src.Split(delimiters, StringSplitOptions.None);
            index = 1;

            //For each part / set of points in the path
            foreach (char c in src.ToCharArray())
            {
                hold = c.ToString();
                cmd = "";
                bool absolute = false;
                if (hold == "") { continue; }

                //Move-To command
                if (hold.ToLower() == "m")
                {
                    //pad with trailing whitespace (this is to help with recognizing the end of the final number in the string)
                    points_str[index] += " ";
                    point_list = points_str[index].Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    //print comment and raise the Z-position to ensure the move is unimpeded
                    cmd += "\r\n(--MOVETO--)";
                    cmd += "\r\nG91Z-0.75;";
                    mi = 0;
                    bi = 0;
                    point = new string[2];

                    //Convert the string to char[] and itterate
                    foreach (char k in points_str[index].ToCharArray())
                    {
                        //If the buffer is not empty and we've reached a delimiter then assume the buffer contains a coordinate value 
                        if ((k == ',' || k == ' ' || k == '-') && buffer[0] != '\0')
                        {
                            if (k == '-' && buffer[bi - 1].ToString().ToLower() == "e")
                            {
                                buffer[bi++] = k;
                            }
                            else
                            {
                                mi++;
                                if (mi % 2 != 0)
                                {
                                    point[0] = new String(buffer).Replace("\0", String.Empty);
                                    buffer = new char[16];
                                    bi = 0;
                                }
                                else
                                {
                                    point[1] = new String(buffer).Replace("\0", String.Empty);
                                    buffer = new char[16];
                                    bi = 0;

                                    //Determine if requested position is absolute or relative and begin building the command
                                    if (hold == "M")
                                    {
                                        cmd += "\r\nG90";
                                        absolute = true;
                                    }
                                    else
                                    {
                                        cmd += "\r\nG91";
                                        absolute = false;
                                    }

                                    cmd += "X" + ScalePoint_X(point[0], absolute) + "Y" + ScalePoint_Y(point[1], absolute);
                                    cmd += ";";

                                    //Update the 'current point' for this instance
                                    if (absolute == true)
                                    {
                                        current_point[0] = ScalePoint_X(point[0], true);
                                        current_point[1] = ScalePoint_Y(point[1], true);
                                    }
                                    else
                                    {
                                        current_point[0] = String.Format("{0:0.00000}", (double.Parse(current_point[0]) + double.Parse(ScalePoint_X(point[0], false))));
                                        current_point[1] = String.Format("{0:0.00000}", (double.Parse(current_point[1]) + double.Parse(ScalePoint_Y(point[1], false))));
                                    }
                                } 

                                //If the current character is '-', then it belongs to the next value, so buffer it
                                if (k == '-')
                                {
                                    buffer[0] = k;
                                    bi = 1;
                                }
                            }
                        }
                        else
                        {
                            //buffer non-space characters
                            if (k != ' ')
                            {
                                buffer[bi++] = k;
                            }
                        }
                    }

                    cmd += "\r\nG91Z0.75;";

                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "l")  //line to
                {
                    //pad with trailing whitespace (this is to help with recognizing the end of the final number in the string)
                    points_str[index] += " ";

                    cmd += "\r\n(--LINETO--)";

                    point_list = points_str[index].Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    mi = 0;
                    bi = 0;
                    point = new string[2];

                    //Convert the string to char[] and itterate
                    foreach (char k in points_str[index].ToCharArray())
                    {
                        //If the buffer is not empty and we've reached a delimiter then assume the buffer contains a coordinate value 
                        if ((k == ',' || k == ' ' || k == '-') && buffer[0] != '\0')
                        {
                            if (k == '-' && buffer[bi - 1].ToString().ToLower() == "e")
                            {
                                buffer[bi++] = k;
                                buffer = new char[16];
                                bi = 0;
                            }
                            else
                            {
                                mi++;
                                if (mi % 2 != 0)
                                {
                                    point[0] = new String(buffer).Replace("\0", String.Empty);
                                    buffer = new char[16];
                                    bi = 0;
                                }
                                else
                                {
                                    point[1] = new String(buffer).Replace("\0", String.Empty);
                                    buffer = new char[16];
                                    bi = 0;

                                    if (hold != hold.ToLower())
                                    {
                                        cmd += "\r\nG90";
                                        absolute = true;
                                    }
                                    else
                                    {
                                        cmd += "\r\nG91";
                                        absolute = false;
                                    }

                                    cmd += "X" + ScalePoint_X(point[0], absolute) + "Y" + ScalePoint_Y(point[1], absolute);
                                    cmd += ";";

                                    if (absolute == true)
                                    {
                                        current_point[0] = ScalePoint_X(point[0], true);
                                        current_point[1] = ScalePoint_Y(point[1], true);
                                    }
                                    else
                                    {
                                        current_point[0] = String.Format("{0:0.00000}", (double.Parse(current_point[0]) + double.Parse(ScalePoint_X(point[0], false))));
                                        current_point[1] = String.Format("{0:0.00000}", (double.Parse(current_point[1]) + double.Parse(ScalePoint_Y(point[1], false))));
                                    }
                                }

                                //If the current character is '-', then it belongs to the next value, so buffer it
                                if (k == '-')
                                {
                                    buffer[0] = k;
                                    bi = 1;
                                }
                            }
                        }
                        else
                        {
                            //buffer non-space characters
                            if (k != ' ')
                            {
                                buffer[bi++] = k;
                            }
                        }
                    }

                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "h") //horizontal line to
                {
                    //pad with trailing whitespace (this is to help with recognizing the end of the final number in the string)
                    points_str[index] += " ";
                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "v") //vertical line to
                {
                    //pad with trailing whitespace (this is to help with recognizing the end of the final number in the string)
                    points_str[index] += " ";
                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "c") //curve to (cubic Bezier curve)
                {
                    //pad with trailing whitespace (this is to help with recognizing the end of the final number in the string)
                    points_str[index] += " ";

                    //A Bezier curve is defined by 'n' points. The first and last points are always the end-points.
                    // the remaining points are controls which provide directional information

                    //parse control points
                    double[] start_point = new double[2];
                    double bnCoeff = 0.0;
                    temp = points_str[index];
                    temp = temp.Replace("e-", "e+");
                    num_points = (temp.Split(new char[] { ' ', ',', '-' }, StringSplitOptions.RemoveEmptyEntries).Count() / 2) + 1;

                    curve_points = new double[num_points, 2];
                    point = new String[2];
                    buffer = new char[16];
                        
                    start_point[0] = double.Parse(current_point[0]);
                    start_point[1] = double.Parse(current_point[1]);

                    ci = 1; //curve point index - starts at 1 because index 0 is set to the current point (below) based on 'absolute' state
                    bi = 0; //buffer index - current index of the array used to buffer the curve point values prior to parsing

                    point[0] = "";
                    point[1] = "";

                    //Determine if the points in the curve are in absolute or relative coordinates
                    if (hold != hold.ToLower())
                    {
                        absolute = true;
                    }
                    else
                    {
                        absolute = false;
                    }
                    curve_points[0, 0] = start_point[0];
                    curve_points[0, 1] = start_point[1];
                        
                    //Print comment line
                    cmd += "\r\n(--CUBIC BEZIER CURVE--)";

                    //Convert the string to char[] and itterate
                    foreach (char k in points_str[index].ToCharArray())
                    {
                        //If the buffer is not empty and we've reached a delimiter then assume the buffer contains a coordinate value 
                        if ((k == ',' || k == ' ' || k == '-') && buffer[0] != '\0')
                        {
                            if(k == '-' && buffer[bi - 1].ToString().ToLower() == "e")
                            {
                                buffer[bi++] = k;
                            }
                            else
                            {
                                //Place the contents of the buffer into the first empty coordinate spot then clear the buffer
                                if (point[0] == "")
                                {
                                    point[0] = new String(buffer).Replace("\0", String.Empty);
                                    buffer = new char[16];
                                    bi = 0;
                                }
                                else if (point[1] == "")
                                {
                                    point[1] = new String(buffer).Replace("\0", String.Empty);
                                    buffer = new char[16];
                                    bi = 0;

                                    //Now that both axis values of the point have been obtained, scale accordingly and place the point into the 
                                    // array of curve points
                                    curve_points[ci, 0] = double.Parse(ScalePoint_X(point[0], absolute));
                                    curve_points[ci, 1] = double.Parse(ScalePoint_Y(point[1], absolute));

                                    //If the coordinates are relative, add to the starting point
                                    if (absolute == false)
                                    {
                                        curve_points[ci, 0] += start_point[0];
                                        curve_points[ci, 1] += start_point[1];

                                        //Each 3rd point after the initial starting point creates a new starting point
                                        // on which the following points are based.
                                        if ((ci % 3) == 0)
                                        {
                                            start_point[0] = curve_points[ci, 0];
                                            start_point[1] = curve_points[ci, 1];
                                        }
                                    }

                                    //increment the curve point counter
                                    ci++;

                                    //cleaer the point array
                                    point[0] = "";
                                    point[1] = "";
                                }

                                //If the current character is '-', then it belongs to the next value, so buffer it
                                if (k == '-')
                                {
                                    buffer[0] = k;
                                    bi = 1;
                                }
                            }
                        }
                        else
                        {
                            //buffer non-space characters
                            if(k != ' ')
                            {
                                buffer[bi++] = k;
                            }
                        }
                    }

                    //Define the resolution for drawing the curve (the number of points).
                    // ie. for t += 0.02 from 0 to 1, the curve will be split into 50 lines
                    for (t = 0; t <= 1; t += 0.02)
                    {
                        //( (1-t)^3 )*P0 + 3*( (1-t)^2 )*t*P1 + 3*(1-t)*(t^2)*P2 + (t^3)*P3;

                        Bezier_Pt[0] = 0;
                        Bezier_Pt[1] = 0;
                        for(int n = 0; n < num_points; n++)
                        {
                            //Calculate the binomial coefficient
                            bnCoeff = 1;
                            for (int i = 1; i <= n; i++)
                            {
                                bnCoeff *= (((double)num_points - (double)i) / (double)i);
                            }

                            Bezier_Pt[0] += Math.Pow((1 - t), num_points - 1 - n) * curve_points[n, 0] * Math.Pow(t, n) * bnCoeff;
                            Bezier_Pt[1] += Math.Pow((1 - t), num_points - 1 - n) * curve_points[n, 1] * Math.Pow(t, n) * bnCoeff;
                        }

                        cmd += "\r\nG90";
                        cmd += "X" + Math.Round(Bezier_Pt[0],4) + "Y" + Math.Round(Bezier_Pt[1],4);
                        cmd += ";";
                        code += cmd;
                        cmd = "";
                    }

                    index++;

                    current_point[0] = String.Format("{0:0.00000}", Bezier_Pt[0]);
                    current_point[1] = String.Format("{0:0.00000}", Bezier_Pt[1]);

                    continue;
                }//****************************************************************//
                else if (hold.ToLower() == "s") //smooth curve to
                {

                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "q") //quadratic Bezier curve
                {

                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "t") //smooth quadratic Bézier curve
                {

                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "a") //elliptical arc
                {

                    index++;
                }//****************************************************************//
                else if (hold.ToLower() == "z") //close path
                {

                    index++;
                }//****************************************************************//

                //Update the G-code to be returned
                code += cmd;
            }
            
            return code;
        }



        /********************************************************************************
         * FUNCTION:        b_lineto
         * Description:     TBC empty template
         * Parameters:  
         *      param -   
         ********************************************************************************/
        private String b_lineto(string param)
        {
            String code = "";
            return code;
        }

        /********************************************************************************
         * FUNCTION:        curveto
         * Description:     TBC empty template
         * Parameters:  
         *      param -   
         ********************************************************************************/
        private String curveto(string param)
        {
            String code = "";
            return code;
        }

    }

}
