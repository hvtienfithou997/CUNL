using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using QLCUNL.BL;
using QLCUNL.Models;

namespace QLCUNL.API.Controllers
{
    //2020
    [Route("api/[controller]")]
    [ApiController]
    public class PhanQuyenController : APIBase
    {
        DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
        public PhanQuyenController()
        {
            dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd-MM-yyyy";
            dtfi.DateSeparator = "-";
        }
        //GET: api/PhanQuyen
        [HttpGet]
        [Route("view")]
        public IActionResult Get(string id)
        {


            var data = QLCUNL.BL.PhanQuyenBL.GetById(id);

            return Ok(new DataResponse() { data = data, success = data != null, msg = "" });
        }
        [HttpPost]
        [Route("share")]
        public IActionResult Share([FromBody] object value)
        {
            //check quyền trước khi share? Chỉ admin + owner mới được quyền share
            bool has_share_permission = (is_sys_admin || is_app_admin);
            DataResponse res = new DataResponse() { msg = "Không có quyền chia sẻ thông tin", success = false };
            try
            {
                var obj = JToken.Parse(value.ToString());
                if (obj != null)
                {
                    List<string> users = obj["user"].ToObject<List<string>>();
                    List<string> lst_id_nha_tuyen_dung = new List<string>();
                    if (obj["id_nha_tuyen_dung"] != null)
                    {
                        string id_nha_tuyen_dung = obj["id_nha_tuyen_dung"].ToString();
                        try
                        {
                            lst_id_nha_tuyen_dung = id_nha_tuyen_dung.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    string id = obj["id"].ToString();
                    int type = obj["type"].ToObject<int>();
                    int rule = obj["rule"].ToObject<int>();
                    int obj_type = obj["obj_type"].ToObject<int>();
                    long ngay_het = 0;
                    List<Quyen> quyen = new List<Quyen>();
                    List<int> teams = new List<int>();

                    if (obj["ngay_het"] != null)
                    {
                        try
                        {
                            ngay_het = XMedia.XUtil.TimeInEpoch(Convert.ToDateTime(ngay_het, dtfi));
                        }
                        catch (Exception)
                        {
                            ngay_het = XMedia.XUtil.TimeInEpoch(DateTime.Now.AddYears(10));
                        }
                    }
                    if (obj["quyen"] != null)
                    {
                        quyen = obj["quyen"].ToObject<List<Quyen>>();
                    }
                    if (!(is_sys_admin || is_app_admin))
                    {
                        var obj_type_check = (PhanQuyenObjType)obj_type;
                        switch (obj_type_check)
                        {
                            case PhanQuyenObjType.CONG_TY:
                                has_share_permission = CongTyBL.IsOwner(id, user);
                                break;
                            case PhanQuyenObjType.USER_JOB:
                                has_share_permission = UserJobBL.IsOwner(id, user);
                                break;
                            case PhanQuyenObjType.NOTE_UNG_VIEN:
                                has_share_permission = NoteUngVienBL.IsOwner(id, user);
                                break;
                            case PhanQuyenObjType.UNG_VIEN:
                                has_share_permission = UngVienBL.IsOwner(id, user);
                                break;
                            case PhanQuyenObjType.JOB:
                                has_share_permission = JobBL.IsOwner(id, user);
                                break;
                            case PhanQuyenObjType.NOTE_UNG_VIEN_JOB:
                                has_share_permission = NoteUngVienJobBL.IsOwner(id, user);
                                break;
                            case PhanQuyenObjType.MENU:
                                has_share_permission = MenuBL.IsOwner(id, user);
                                break;
                        }

                    }
                    if (!has_share_permission)
                        return Ok(res);

                    if (obj["teams"] != null)
                    {
                        try
                        {
                            teams = obj["teams"].ToObject<List<int>>();
                        }
                        catch
                        {
                            teams = new List<int>();
                        }
                    }

                    #region Xóa shared của các đối tượng cũ
                    try
                    {
                        List<string> lst_id_phan_quyen_can_xoa = new List<string>();
                        var menu_shared = QLCUNL.BL.PhanQuyenBL.Get(string.Empty, PhanQuyenRule.ALL, PhanQuyenType.ALL, string.Empty, PhanQuyenObjType.ALL, id, quyen, 0, 0, ((is_sys_admin || is_app_admin) ? string.Empty : user), 0, 0, string.Empty, 0, 0, 1, 9999, out _);
                        foreach (var item_shared in menu_shared)
                        {
                            if (item_shared.type == PhanQuyenType.GROUP_USERS)
                            {
                                int id_team_shared = -1;

                                if (Int32.TryParse(item_shared.user, out id_team_shared))
                                {
                                    if (!teams.Contains(id_team_shared))
                                    {
                                        lst_id_phan_quyen_can_xoa.Add(item_shared.id);
                                    }
                                    else
                                    {
                                        if (item_shared.ngay_het == ngay_het && item_shared.quyen.All(quyen.Contains) && quyen.All(item_shared.quyen.Contains))
                                            teams.Remove(id_team_shared);
                                    }
                                }

                            }
                            else
                            {
                                if (item_shared.type == PhanQuyenType.USERS)
                                {
                                    if (!users.Contains(item_shared.user) && !lst_id_nha_tuyen_dung.Contains(item_shared.user))
                                    {
                                        lst_id_phan_quyen_can_xoa.Add(item_shared.id);
                                    }
                                    else
                                    {

                                        if (item_shared.ngay_het == ngay_het && item_shared.quyen.All(quyen.Contains) && quyen.All(item_shared.quyen.Contains))
                                            users.Remove(item_shared.user);

                                    }
                                }
                            }
                        }
                        PhanQuyenBL.RemoveByListId(lst_id_phan_quyen_can_xoa);
                    }
                    catch (Exception)
                    {
                    }
                    #endregion
                    #region Shared cho các đối tượng được chọn
                    try
                    {
                        if (users.Count == 0 && teams.Count == 0 && lst_id_nha_tuyen_dung.Count == 0)
                        {
                            res.success = true;
                            res.msg = "Không có thay đổi nào được thực hiện";
                        }
                        else
                        {
                            foreach (var user in users)
                            {
                                PhanQuyen pq = new PhanQuyen();
                                pq.ngay_het = ngay_het;
                                pq.user = user;
                                pq.type = PhanQuyenType.USERS;
                                pq.rule = (PhanQuyenRule)rule;
                                pq.quyen = quyen;
                                pq.obj_type = (PhanQuyenObjType)obj_type;
                                pq.nguoi_tao = this.user;
                                pq.obj_id = id;

                                res.success = QLCUNL.BL.PhanQuyenBL.Index(pq);
                            }
                            foreach (var user in lst_id_nha_tuyen_dung)
                            {
                                PhanQuyen pq = new PhanQuyen();
                                pq.ngay_het = ngay_het;
                                pq.user = user;
                                pq.type = PhanQuyenType.USERS;
                                pq.rule = (PhanQuyenRule)rule;
                                pq.quyen = quyen;
                                pq.obj_type = (PhanQuyenObjType)obj_type;
                                pq.nguoi_tao = this.user;
                                pq.obj_id = id;

                                res.success = QLCUNL.BL.PhanQuyenBL.Index(pq);
                            }
                            if (teams.Count > 0)
                            {
                                foreach (var id_team in teams)
                                {
                                    PhanQuyen pq = new PhanQuyen();
                                    pq.ngay_het = ngay_het;
                                    pq.user = id_team.ToString();
                                    pq.type = PhanQuyenType.GROUP_USERS;
                                    pq.rule = (PhanQuyenRule)rule;
                                    pq.quyen = quyen;
                                    pq.nguoi_tao = user;
                                    pq.obj_type = (PhanQuyenObjType)obj_type;
                                    pq.obj_id = id;
                                    res.success = QLCUNL.BL.PhanQuyenBL.Index(pq);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }
        //POST: api/PhanQuyen
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var phan_quyen = Newtonsoft.Json.JsonConvert.DeserializeObject<PhanQuyen>(value.ToString());
                res.success = QLCUNL.BL.PhanQuyenBL.Index(phan_quyen);
            }
            catch (Exception ex)
            {
                res.msg = ex.Message; res.success = false;
            }

            return Ok(res);
        }

        // PUT: api/PhanQuyen/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var phan_quyen = Newtonsoft.Json.JsonConvert.DeserializeObject<PhanQuyen>(value.ToString());
                phan_quyen.id = id;
                res.success = QLCUNL.BL.PhanQuyenBL.Update(phan_quyen);
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete()]
        [Route("delete")]
        public void Delete(string id)
        {

        }
        [HttpPut]
        [Route("updatengayhethan")]
        public IActionResult UpdateNgayHetHan([FromBody] object value)
        {
            DataResponse res = new DataResponse();
            try
            {
                var obj = JToken.Parse(value.ToString());
                if (obj != null)
                {
                    var lst = obj["data"].ToObject<List<PhanQuyen>>();
                    string nguoi_sua = obj["nguoi_sua"].ToString();

                    res.success = QLCUNL.BL.PhanQuyenBL.UpdateNgayHetHan(lst, nguoi_sua);
                }
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }
        [HttpGet]
        [Route("getbyuser")]
        public IActionResult GetByUser(string user)
        {
            DataResponse res = new DataResponse();
            try
            {
                res.data = QLCUNL.BL.PhanQuyenBL.GetByUser(user);
                res.success = true;

            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }
        [HttpGet]
        [Route("getquyenactive")]
        public IActionResult GetQuyenActive(string user, int group, PhanQuyenObjType obj_type, string quyen)
        {
            string[] fields = new string[] { };
            DataResponse res = new DataResponse();
            try
            {
                res.data = QLCUNL.BL.PhanQuyenBL.GetQuyenActive(user, group, obj_type, quyen.Split(',').Select(x => Convert.ToInt32(x)), fields);
                res.success = true;

            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }
        [HttpGet]
        [Route("get")]
        public IActionResult Get(string term, PhanQuyenRule rule, PhanQuyenType type, string user,
           PhanQuyenObjType obj_type, string obj_id, List<Quyen> quyen,
           long ngay_het_tu, long ngay_het_den, string nguoi_tao, long ngay_tao_tu, long ngay_tao_den,
           string nguoi_sua, long ngay_sua_tu, long ngay_sua_den, int page, int recs)
        {
            long total;
            DataResponsePaging res = new DataResponsePaging();
            try
            {
                res.data = QLCUNL.BL.PhanQuyenBL.Get(term, rule, type, user, obj_type, obj_id, quyen, ngay_het_tu, ngay_het_den, nguoi_tao, ngay_tao_tu, ngay_tao_den, nguoi_sua, ngay_sua_tu, ngay_sua_den, page, recs, out total);
                res.success = true;
                res.total = total;
            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }
        [HttpGet]
        [Route("isexistquyen")]
        public IActionResult IsExistQuyen(string user, int group, PhanQuyenObjType obj_type, string obj_id, string owner, List<int> quyen)
        {
            DataResponse res = new DataResponse();
            try
            {
                res.data = QLCUNL.BL.PhanQuyenBL.IsExistQuyen(user, group, obj_type, obj_id, owner, quyen);
                res.success = true;

            }
            catch (Exception ex)
            {
                res.msg = ex.StackTrace; res.success = false;
            }

            return Ok(res);
        }
    }
}