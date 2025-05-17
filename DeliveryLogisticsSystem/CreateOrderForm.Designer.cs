namespace DeliveryLogisticsSystem
{
    partial class CreateOrderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateOrderForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnBack = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSubmitOrder = new System.Windows.Forms.Button();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPickUpAddress = new System.Windows.Forms.TextBox();
            this.txtDropOffName = new System.Windows.Forms.TextBox();
            this.txtDropOffAddress = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.WBBMap = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WBBMap)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.MaximumSize = new System.Drawing.Size(296, 600);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(296, 600);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(296, 600);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnBack
            // 
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.Location = new System.Drawing.Point(320, 25);
            this.btnBack.MaximumSize = new System.Drawing.Size(35, 35);
            this.btnBack.MinimumSize = new System.Drawing.Size(35, 35);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(35, 35);
            this.btnBack.TabIndex = 1;
            this.btnBack.TabStop = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(370, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 41);
            this.label1.TabIndex = 2;
            this.label1.Text = "Create Order";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(316, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Email";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(316, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Pick-Up Location Address";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(316, 307);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(188, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Drop-Off Location Address";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(316, 245);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Drop-Off Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(316, 431);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 20);
            this.label6.TabIndex = 7;
            this.label6.Text = "Price";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(316, 369);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 20);
            this.label7.TabIndex = 8;
            this.label7.Text = "Phone Number";
            // 
            // btnSubmitOrder
            // 
            this.btnSubmitOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(119)))), ((int)(((byte)(67)))));
            this.btnSubmitOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmitOrder.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnSubmitOrder.Location = new System.Drawing.Point(320, 540);
            this.btnSubmitOrder.MaximumSize = new System.Drawing.Size(155, 35);
            this.btnSubmitOrder.MinimumSize = new System.Drawing.Size(155, 35);
            this.btnSubmitOrder.Name = "btnSubmitOrder";
            this.btnSubmitOrder.Size = new System.Drawing.Size(155, 35);
            this.btnSubmitOrder.TabIndex = 9;
            this.btnSubmitOrder.Text = "Submit";
            this.btnSubmitOrder.UseVisualStyleBackColor = false;
            this.btnSubmitOrder.Click += new System.EventHandler(this.btnSubmitOrder_Click);
            // 
            // txtEmail
            // 
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtEmail.Location = new System.Drawing.Point(320, 136);
            this.txtEmail.MaximumSize = new System.Drawing.Size(317, 32);
            this.txtEmail.MinimumSize = new System.Drawing.Size(317, 32);
            this.txtEmail.Multiline = true;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(317, 32);
            this.txtEmail.TabIndex = 11;
            // 
            // txtPickUpAddress
            // 
            this.txtPickUpAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPickUpAddress.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtPickUpAddress.Location = new System.Drawing.Point(320, 203);
            this.txtPickUpAddress.MaximumSize = new System.Drawing.Size(317, 32);
            this.txtPickUpAddress.MinimumSize = new System.Drawing.Size(317, 32);
            this.txtPickUpAddress.Multiline = true;
            this.txtPickUpAddress.Name = "txtPickUpAddress";
            this.txtPickUpAddress.Size = new System.Drawing.Size(317, 32);
            this.txtPickUpAddress.TabIndex = 12;
            // 
            // txtDropOffName
            // 
            this.txtDropOffName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDropOffName.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtDropOffName.Location = new System.Drawing.Point(320, 270);
            this.txtDropOffName.MaximumSize = new System.Drawing.Size(317, 32);
            this.txtDropOffName.MinimumSize = new System.Drawing.Size(317, 32);
            this.txtDropOffName.Multiline = true;
            this.txtDropOffName.Name = "txtDropOffName";
            this.txtDropOffName.Size = new System.Drawing.Size(317, 32);
            this.txtDropOffName.TabIndex = 13;
            // 
            // txtDropOffAddress
            // 
            this.txtDropOffAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDropOffAddress.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtDropOffAddress.Location = new System.Drawing.Point(320, 332);
            this.txtDropOffAddress.MaximumSize = new System.Drawing.Size(317, 32);
            this.txtDropOffAddress.MinimumSize = new System.Drawing.Size(317, 32);
            this.txtDropOffAddress.Multiline = true;
            this.txtDropOffAddress.Name = "txtDropOffAddress";
            this.txtDropOffAddress.Size = new System.Drawing.Size(317, 32);
            this.txtDropOffAddress.TabIndex = 14;
            // 
            // txtPhone
            // 
            this.txtPhone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtPhone.Location = new System.Drawing.Point(320, 394);
            this.txtPhone.MaximumSize = new System.Drawing.Size(317, 32);
            this.txtPhone.MinimumSize = new System.Drawing.Size(317, 32);
            this.txtPhone.Multiline = true;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(317, 32);
            this.txtPhone.TabIndex = 15;
            // 
            // txtPrice
            // 
            this.txtPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrice.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.txtPrice.Location = new System.Drawing.Point(320, 456);
            this.txtPrice.MaximumSize = new System.Drawing.Size(128, 32);
            this.txtPrice.MinimumSize = new System.Drawing.Size(128, 32);
            this.txtPrice.Multiline = true;
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(128, 32);
            this.txtPrice.TabIndex = 16;
            // 
            // WBBMap
            // 
            this.WBBMap.AllowExternalDrop = true;
            this.WBBMap.CreationProperties = null;
            this.WBBMap.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WBBMap.Dock = System.Windows.Forms.DockStyle.Right;
            this.WBBMap.Location = new System.Drawing.Point(709, 0);
            this.WBBMap.MaximumSize = new System.Drawing.Size(469, 600);
            this.WBBMap.MinimumSize = new System.Drawing.Size(469, 600);
            this.WBBMap.Name = "WBBMap";
            this.WBBMap.Size = new System.Drawing.Size(469, 600);
            this.WBBMap.TabIndex = 17;
            this.WBBMap.ZoomFactor = 1D;
            // 
            // CreateOrderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(1178, 584);
            this.Controls.Add(this.WBBMap);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.txtDropOffAddress);
            this.Controls.Add(this.txtDropOffName);
            this.Controls.Add(this.txtPickUpAddress);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.btnSubmitOrder);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.MaximumSize = new System.Drawing.Size(1200, 640);
            this.MinimumSize = new System.Drawing.Size(1200, 640);
            this.Name = "CreateOrderForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreateOrderForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WBBMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox btnBack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSubmitOrder;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtPickUpAddress;
        private System.Windows.Forms.TextBox txtDropOffName;
        private System.Windows.Forms.TextBox txtDropOffAddress;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtPrice;
        private Microsoft.Web.WebView2.WinForms.WebView2 WBBMap;
    }
}