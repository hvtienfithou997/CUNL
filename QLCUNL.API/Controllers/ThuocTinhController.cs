using Microsoft.AspNetCore.Mvc;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLCUNL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThuocTinhController : APIBase
    {
        [HttpPost]
        [Route("all")]
        public IActionResult All([FromBody] BaseGetAll res)
        {
            List<ThuocTinh> ThuocTinh = QLCUNL.BL.ThuocTinhBL.GetAll(res.page_index, res.page_size).ToList();

            return Ok(new DataResponse() { data = ThuocTinh, success = ThuocTinh != null, msg = "" });
        }

        //GET: api/ThuocTinh
        [HttpGet()]
        [Route("view")]
        public IActionResult Get(string id)
        {
            var ThuocTinh = QLCUNL.BL.ThuocTinhBL.GetById(id);

            return Ok(new DataResponse() { data = ThuocTinh, success = ThuocTinh != null, msg = "" });
        }

        //POST: api/ThuocTinh
        [HttpPost]
        [Route("add")]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ThuocTinh>(value.ToString());
                if (!QLCUNL.BL.ThuocTinhBL.IsTenThuocTinhExist(app_id, data.loai, data.type, data.ten))
                {
                    if ((is_app_admin || is_sys_admin) && !string.IsNullOrEmpty(data.nguoi_tao))
                    {
                        data.app_id = app_id;
                        data.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                        data.nguoi_tao = data.nguoi_tao;
                        data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                        data.nguoi_sua = data.nguoi_tao;
                    }
                    else
                        SetMetaData(data, false);

                    //Không phải SYS_ADMIN thì chỉ được tạo thuộc tính dạng PRIVATE (CÁ NHÂN DÙNG)
                    if (data.type == ThuocTinhType.SHARED && !(is_sys_admin || is_app_admin))
                    {
                        data.type = ThuocTinhType.PRIVATE;
                    }

                    res.success = QLCUNL.BL.ThuocTinhBL.Index(data);
                }
                else
                {
                    res.success = false;
                    res.msg = "Tên thuộc tính cho đối tượng này đã tồn tại";
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        // PUT: api/ThuocTinh/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                string msg = "";
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ThuocTinh>(value.ToString());
                data.id = id;
                if ((is_app_admin || is_sys_admin) && !string.IsNullOrEmpty(data.nguoi_tao))
                {
                    data.app_id = app_id;
                    data.ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                    data.nguoi_tao = data.nguoi_tao;
                    data.ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
                    data.nguoi_sua = data.nguoi_tao;
                }
                else
                    SetMetaData(data, true);

                //Không phải SYS_ADMIN thì chỉ được tạo thuộc tính dạng PRIVATE (CÁ NHÂN DÙNG)
                if (!(is_sys_admin || is_app_admin))
                {
                    data.type = ThuocTinhType.PRIVATE;
                    SetMetaData(data, true);
                    res.success = QLCUNL.BL.ThuocTinhBL.UpdateTenNhom(data);
                }
                else
                {
                    res.success = QLCUNL.BL.ThuocTinhBL.Update(data, out msg);
                }

                res.msg = msg;
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        [Route("delete")]
        public bool Delete(string id)
        {
            if (is_app_admin || QLCUNL.BL.ThuocTinhBL.IsOwner(id, user))
                return QLCUNL.BL.ThuocTinhBL.Delete(id);
            return false;
        }

        [HttpGet]
        [Route("loai")]
        public IActionResult GetAllByLoai(int id, int type = (int)ThuocTinhType.SHARED)
        {
            DataResponse res = new DataResponse();
            try
            {
                res.data = is_app_admin ? QLCUNL.BL.ThuocTinhBL.GetAllByLoaiThuocTinh(app_id, id, type).OrderBy(x => x.nhom) : QLCUNL.BL.ThuocTinhBL.GetAllByLoaiThuocTinh(app_id, user, id).OrderBy(x => x.nhom);

                res.success = res.data != null;
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string term, int loai, int type, int page, int page_size)
        {
            long total_recs;
            string msg;
            List<ThuocTinh> lst = new List<ThuocTinh>();

            lst = QLCUNL.BL.ThuocTinhBL.Search(app_id, user, term, loai, type, page, out total_recs, out msg, page_size, (is_sys_admin || is_app_admin));
            return Ok(new DataResponsePaging() { data = lst.OrderBy(x => x.nhom), total = total_recs, success = lst != null, msg = msg });
        }

        [HttpGet]
        [Route("canhan")]
        public IActionResult GetAllByLoaiCaNhan(string term, int loai, string id = "")
        {
            DataResponseExt res = new DataResponseExt();
            try
            {
                var tupe = GetByLoai(term, loai, id);
                var shared_data = tupe.Item1;
                var thuoc_tinh_da_luu = tupe.Item2;
                List<dynamic> lst_kvp_data_luu = new List<dynamic>();
                foreach (var item in thuoc_tinh_da_luu)
                {
                    lst_kvp_data_luu.Add(new { k = item, v = 0 });
                }
                var private_data = QLCUNL.BL.ThuocTinhBL.Search(app_id, user, term, loai, (int)ThuocTinhType.PRIVATE, 1, out long total_recs, out string msg, 9999, (is_sys_admin || is_app_admin));
                var list = private_data.Concat(shared_data).ToList();
                res.data = list.OrderBy(x => x.nhom);


                if (!string.IsNullOrEmpty(id))
                {
                    string id_full = $"{((LoaiThuocTinh)loai).ToString()}_{id}";
                    var gia_tri_thuoc_tinh_rieng_cu = QLCUNL.BL.ThuocTinhDuLieuBL.GetIdThuocTinhByIdObj(app_id, id, (LoaiThuocTinh)loai, user);
                    if (gia_tri_thuoc_tinh_rieng_cu != null)
                    {
                        foreach (var item in gia_tri_thuoc_tinh_rieng_cu)
                        {
                            lst_kvp_data_luu.Add(new { k = item, v = 1 });
                        }
                    }
                }
                res.value = lst_kvp_data_luu;
                res.success = res.data != null;
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        [HttpGet]
        [Route("shared")]
        public IActionResult GetAllByLoaiShared(string term, int loai, string id = "")
        {
            DataResponseExt res = new DataResponseExt();
            try
            {
                res.data = QLCUNL.BL.ThuocTinhBL.Search(app_id, string.Empty, term, loai, (int)ThuocTinhType.SHARED, 1, out long total_recs, out string msg).OrderBy(x => x.nhom);
                res.value = new List<int>();
                if (!string.IsNullOrEmpty(id))
                {
                    List<int> thuoc_tinh = new List<int>();
                    switch ((LoaiThuocTinh)loai)
                    {
                        case LoaiThuocTinh.CONG_TY:
                            var cong_ty = QLCUNL.BL.CongTyBL.GetById(id);
                            if (cong_ty != null)
                            {
                                thuoc_tinh = cong_ty.thuoc_tinh;
                            }
                            break;

                        case LoaiThuocTinh.USER_JOB:
                            var user_job = QLCUNL.BL.UserJobBL.GetById(id);
                            if (user_job != null)
                            {
                                thuoc_tinh = user_job.thuoc_tinh;
                            }
                            break;

                        case LoaiThuocTinh.NOTE_UNG_VIEN:
                            var note_ung_vien = QLCUNL.BL.NoteUngVienBL.GetById(id);
                            if (note_ung_vien != null)
                            {
                                thuoc_tinh = note_ung_vien.thuoc_tinh;
                            }
                            break;

                        case LoaiThuocTinh.UNG_VIEN:
                            var ung_vien = QLCUNL.BL.UngVienBL.GetById(id);
                            if (ung_vien != null)
                            {
                                thuoc_tinh = ung_vien.thuoc_tinh;
                            }
                            break;

                        case LoaiThuocTinh.JOB:
                            var job = QLCUNL.BL.JobBL.GetById(id);
                            if (job != null)
                            {
                                thuoc_tinh = job.thuoc_tinh;
                            }
                            break;

                        case LoaiThuocTinh.NOTE_UNG_VIEN_JOB:
                            var note_ung_vien_job = QLCUNL.BL.NoteUngVienJobBL.GetById(id);
                            if (note_ung_vien_job != null)
                            {
                                thuoc_tinh = note_ung_vien_job.thuoc_tinh;
                            }
                            break;
                    }

                    res.value = thuoc_tinh ?? new List<int>();
                }
                res.success = res.data != null;
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        public Tuple<List<ThuocTinh>, List<int>> GetByLoai(string term, int loai, string id = "")
        {


            var data = BL.ThuocTinhBL.Search(app_id, string.Empty, term, loai, (int)ThuocTinhType.SHARED, 1, out long total_recs, out string msg);
            List<int> thuoc_tinh = new List<int>();
            if (!string.IsNullOrEmpty(id))
            {

                switch ((LoaiThuocTinh)loai)
                {
                    case LoaiThuocTinh.CONG_TY:
                        var cong_ty = QLCUNL.BL.CongTyBL.GetById(id);
                        if (cong_ty != null)
                        {
                            thuoc_tinh = cong_ty.thuoc_tinh;
                        }
                        break;

                    case LoaiThuocTinh.USER_JOB:
                        var user_job = QLCUNL.BL.UserJobBL.GetById(id);
                        if (user_job != null)
                        {
                            thuoc_tinh = user_job.thuoc_tinh;
                        }
                        break;

                    case LoaiThuocTinh.NOTE_UNG_VIEN:
                        var note_ung_vien = QLCUNL.BL.NoteUngVienBL.GetById(id);
                        if (note_ung_vien != null)
                        {
                            thuoc_tinh = note_ung_vien.thuoc_tinh;
                        }
                        break;

                    case LoaiThuocTinh.UNG_VIEN:
                        var ung_vien = QLCUNL.BL.UngVienBL.GetById(id);
                        if (ung_vien != null)
                        {
                            thuoc_tinh = ung_vien.thuoc_tinh;
                        }
                        break;

                    case LoaiThuocTinh.JOB:
                        var job = QLCUNL.BL.JobBL.GetById(id);
                        if (job != null)
                        {
                            thuoc_tinh = job.thuoc_tinh;
                        }
                        break;

                    case LoaiThuocTinh.NOTE_UNG_VIEN_JOB:
                        var note_ung_vien_job = QLCUNL.BL.NoteUngVienJobBL.GetById(id);
                        if (note_ung_vien_job != null)
                        {
                            thuoc_tinh = note_ung_vien_job.thuoc_tinh;
                        }
                        break;
                }
            }
            return new Tuple<List<ThuocTinh>, List<int>>(data, thuoc_tinh == null ? new List<int>() : thuoc_tinh);

        }
    }

}